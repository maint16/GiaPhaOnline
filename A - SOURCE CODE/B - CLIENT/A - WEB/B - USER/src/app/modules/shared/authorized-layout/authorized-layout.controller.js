module.exports = function (ngModule) {
    ngModule.controller('authorizedLayoutController',
        function (oAuthSettings, appSettings, notificationCategoryConstant, notificationActionConstant,
                  $scope, $state, $transitions, uiService, oAuthService,
                  profile, $uibModal, $timeout, $window, $translate, toastr,
                  notificationStatusConstant, userRoleConstant, realTimeChannelConstant, realTimeEventConstant, pusherOptionConstant, hubConstant,
                  pusherService, realTimeService,
                  authenticationService, userService, postNotificationService, postService) {

            //#region Properties

            // Resolver reflection.
            $scope.profile = profile;

            // Modal dialogs list.
            $scope.modals = {
                login: null,
                basicUserRegistration: null
            };

            // Whether google login has been loaded or not.
            $scope.bIsGoogleLoginLoaded = false;
            $scope.bIsFacebookLoginLoaded = false;

            // Temporary data which is for caching.
            $scope.buffer = {
                users: {},
                posts: {},
                initializedRealTimeChannels: {}
            };

            // Search result buffer.
            $scope.result = {
                postNotifications: []
            };

            //#endregion

            //#region Methods

            /*
            * Callback which is called when component starts being initiated.
            * */
            $scope.init = function () {

                // Add google sdk to page.
                oAuthService.addGoogleSdk('fnGoogleClientInitialized');

                // Add facebook sdk to page.
                oAuthService.addFacebookSdk();

                // Load post notifications.
                $scope.fnLoadPostNotifications();

                // Subscribe to real-time channels.
                // $scope.fnSubscribeRealTimeChannels();

                // Subscribe real-time hub.
                $scope.fnSubscribeSignalrConnection();
            };

            /*
            * Callback which is fired when basic login button is clicked.
            * */
            $scope.fnClickLogin = function () {
                // Display basic login modal.
                $scope.modals.login = $uibModal.open({
                    templateUrl: 'basic-login.html',
                    scope: $scope,
                    size: 'lg'
                });
            };

            /*
            * Callback which is fired when login successfully.
            * */
            $scope.fnBasicLogin = function (model) {

                userService.basicLogin(model)
                    .then(
                        function success(basicLoginResponse) {

                            // Login result.
                            var basicLoginResult = basicLoginResponse.data;

                            // Save access token into storage.
                            authenticationService.initAuthenticationToken(basicLoginResult.code);

                            // Dismiss the modal.
                            if ($scope.modals.login) {
                                $scope.modals.login.dismiss();
                                $scope.modals.login = null;
                            }

                            // Reload the state.
                            $state.reload();
                        },
                        function error(basicLoginResponse) {
                            // $scope.ngLoginFailingly();
                        });

            };

            /*
            * Callback which is fired when google login is clicked.
            * */
            $scope.fnGoogleLogin = function () {

                // Close modal dialog.
                if ($scope.modals.login) {
                    $scope.modals.login.dismiss();
                    $scope.modals.login = null;
                }

                var pGoogleAuthenticationClient = gapi.auth2.getAuthInstance();
                pGoogleAuthenticationClient
                    .grantOfflineAccess({
                        scope: 'profile email'
                    })
                    .then(function (getGoogleCredentialResponse) {
                        var szCode = getGoogleCredentialResponse.code;
                        userService.fnUseGoogleLogin({code: szCode})
                            .then(
                                function (loginResponse) {

                                    var loginResult = loginResponse.data;
                                    if (!loginResult)
                                        return;

                                    var szAccessToken = loginResult.accessToken;
                                    if (!szAccessToken || szAccessToken.length < 1)
                                        return;

                                    // Save access token to local storage.
                                    authenticationService.initAuthenticationToken(szAccessToken);

                                    // Reload the current state.
                                    $state.reload();
                                });
                    });
            };

            /*
            * Callback which is fired when facebook login is clicked.
            * */
            $scope.fnFacebookLogin = function () {

                // Close modal dialog.
                if ($scope.modals.login) {
                    $scope.modals.login.dismiss();
                    $scope.modals.login = null;
                }

                // Sign user into system.
                FB.login(function (response) {
                    console.log(response);

                    // Not connected to facebook api.
                    var szStatus = response.status;
                    if (szStatus !== 'connected')
                        return;

                    var authResponse = response.authResponse;
                    if (!authResponse)
                        return;

                    var szAccessToken = authResponse.accessToken;
                    if (!szAccessToken)
                        return;


                    userService.fnUseFacebookLogin({code: szAccessToken})
                        .then(function (loginResponse) {
                            var loginResult = loginResponse.data;
                            if (!loginResult)
                                return;

                            var szAccessToken = loginResult.accessToken;
                            if (!szAccessToken || szAccessToken.length < 1)
                                return;

                            // Save access token to local storage.
                            authenticationService.initAuthenticationToken(szAccessToken);

                            // Reload the current state.
                            $state.reload();
                        });

                }, {scope: 'public_profile,email'});
            };

            /*
            * Event which is fired when sign out button is clicked.
            * */
            $scope.fnSignOut = function () {
                // Clear access token.
                authenticationService.clearAuthenticationToken();

                // Reload the current state.
                $state.reload();
            };

            /*
            * Event which is fired when cancel button is clicked.
            * */
            $scope.fnCancelLogin = function () {
                if (!$scope.modals.login) {
                    return;
                }

                $scope.modals.login.dismiss();
                $scope.modals.login = null;
            };

            /*
            * Function is called when basic user registration form cancel button is clicked.
            * */
            $scope.fnCancelBasicRegister = function () {

                // User registration modal is valid. Dismiss it.
                if ($scope.modals.basicUserRegistration) {
                    $scope.modals.basicUserRegistration.dismiss();
                    $scope.modals.basicUserRegistration = null;
                }
            };

            /*
            * Event which is fired when basic registration button is clicked.
            * */
            $scope.fnOpenBasicRegister = function () {

                // Dismiss the login modal first.
                if ($scope.modals.login)
                    $scope.modals.login.dismiss();

                // Open basic user registration box.
                if ($scope.modals.basicUserRegistration)
                    $scope.modals.basicUserRegistration = $uibModal.open({
                        templateUrl: 'basic-register.html',
                        scope: $scope,
                        size: 'lg'
                    });
            };

            /*
            * Submit a request with specific information to register a basic account.
            * */
            $scope.fnBasicRegister = function (user) {
                userService.basicRegister(user)
                    .then(function (basicUserRegistrationResponse) {
                        // User registration is successful.
                        // Modal dialog is valid. Dismiss it first.
                        if ($scope.modals.basicUserRegistration)
                            $scope.modals.basicUserRegistration.dismiss();
                    });
            };

            /*
            * Event which is fired when Google SDK has been loaded.
            * */
            $window.fnGoogleClientInitialized = function () {

                gapi.load('auth2', function () {

                    // Google login script has been loaded.
                    $scope.bIsGoogleLoginLoaded = true;

                    var params = {
                        client_id: oAuthSettings.google.clientId,
                        scope: 'email',
                        fetch_basic_profile: true
                    };

                    gapi.auth2.init(params);
                });
            };

            /*
            Event which is fired when facebook sdk has been initiated successfully.
            */
            $window.fbAsyncInit = function () {

                // Enable facebook o-auth.
                $scope.bIsFacebookLoginLoaded = true;

                FB.init({
                    appId: oAuthSettings.facebook.clientId,
                    cookie: true,  // enable cookies to allow the server to access
                                   // the session
                    xfbml: true,  // parse social plugins on this page
                    version: 'v2.12' // use graph api version 2.8
                });
            };

            /*
            * Load post notifications list.
            * */
            $scope.fnLoadPostNotifications = function () {

                // User is not authorized.
                if (!$scope.profile)
                    return;

                // Build the load condition.
                var getPostNotificationCondition = {
                    statuses: [notificationStatusConstant.unseen],
                    pagination: {
                        page: 1,
                        records: appSettings.pagination.postNotifications
                    }
                };

                // Search post notification by using specific conditions.
                postNotificationService.search(getPostNotificationCondition)
                    .then(function (getPostNotificationResponse) {
                        // Get post notification result.
                        var getPostNotificationResult = getPostNotificationResponse.data;

                        if (!getPostNotificationResult)
                            throw 'Cannot get post notifications';

                        var notifications = getPostNotificationResult.records;
                        if (!notifications || notifications.length < 1) {
                            $scope.result.postNotifications = getPostNotificationResult;
                            throw 'No post notification for the current user.';
                        }

                        return getPostNotificationResult;
                    })
                    .then(function (getPostNotificationResult) {

                        // Get list of notifications.
                        var notifications = getPostNotificationResult.records;

                        // Promises list to be complete.
                        var promises = [];

                        //#region Get users promises

                        // Build a list of users that don't exist in buffer.
                        var userIds = [];

                        // Build a list of posts that don't exist in buffer.
                        var postIds = [];

                        // Find a list of users that are not in buffer.
                        angular.forEach(notifications, function (notification, iterator) {

                            // Owner is not in buffer.
                            if (!$scope.buffer.users[notification.ownerId]) {
                                userIds.push(notification.ownerId);
                            }

                            // Build a list of posts that are not in buffer.
                            if (!$scope.buffer.posts[notification.postId]) {
                                postIds.push(notification.postId);
                            }

                        });

                        //#region Load user

                        // Build promises to load users.
                        var getUsersCondition = {
                            ids: userIds
                        };

                        // Build promise.
                        var getUsersPromise = userService.loadUsers(getUsersCondition)
                            .then(function (getUsersResponse) {
                                // Get response result.
                                var getUsersResult = getUsersResponse.data;

                                if (!getUsersResult)
                                    return;

                                // Get users list.
                                var users = getUsersResult.records;

                                // List is empty.
                                if (!users || users.length < 1)
                                    return;

                                // Add user to buffer.
                                angular.forEach(users, function (user, iterator) {
                                    $scope.buffer.users[user.id] = user;
                                });
                            });

                        // Add promise to list.
                        promises.push(getUsersPromise);

                        //#endregion

                        //#region Load posts

                        // Build post loading condition.
                        var getPostsCondition = {
                            ids: postIds
                        };

                        // Build get posts promise.
                        var getPostsPromise = postService.loadPosts(getPostsCondition)
                            .then(function (getPostsResponse) {

                                // Get response result.
                                var getPostsResult = getPostsResponse.data;
                                if (!getPostsResult)
                                    return;

                                // Get posts list.
                                var posts = getPostsResult.records;
                                if (!posts || posts.length < 1)
                                    return;

                                // Add post to buffer.
                                angular.forEach(posts, function (post, iterator) {
                                    $scope.buffer.posts[post.id] = post;
                                });
                            });

                        // Add promise to array.
                        promises.push(getPostsPromise);

                        //#endregion

                        // Wait for all promises to be resolved.
                        return Promise.all(promises).then(function () {
                            return getPostNotificationResult;
                        });
                    })
                    .then(function (getPostNotificationResult) {
                        $scope.result.postNotifications = getPostNotificationResult;
                    });

            };

            // /*
            // * Subscribe to realtime channels.
            // * */
            // $scope.fnSubscribeRealTimeChannels = function () {
            //     // // Initialize socket.
            //     var socket = new Pusher(pusherOptionConstant.appKey, {
            //         cluster: pusherOptionConstant.cluster,
            //         encrypted: pusherOptionConstant.encrypted,
            //         authorizer: function (channel, options) {
            //             return {
            //                 authorize: function (socketId, callback) {
            //                     pusherService.authorizeRealTimeChannel(channel.name, socketId)
            //                         .then(function(authorizeClientDeviceResponse){
            //                            var authorizeClientDeviceResult = authorizeClientDeviceResponse.data;
            //                            if (!authorizeClientDeviceResult)
            //                                return;
            //
            //                             callback(false, authorizeClientDeviceResult);
            //                         });
            //                 }
            //             };
            //         }
            //     });
            //
            //     socket.subscribe('private-message-channel')
            //         .bind('my-event', function(data) {
            //             alert('An event was triggered with message: ' + data.message);
            //         });
            //
            //     // Save the socket connection.
            //     pusherService.setInstance(socket);
            // };

            /*
            * Subscribe to signalr connection.
            * */
            $scope.fnSubscribeSignalrConnection = function(){

                // Get hub name constants.
                var hubNameConstant = hubConstant.hubName;

                var parameters = {
                    accessToken: authenticationService.getAuthenticationToken()
                };

                // Add signalr hubs declaration.
                var notificationHubConnection = realTimeService.addHub(hubNameConstant.notificationHub, parameters);
                notificationHubConnection.on(hubConstant.hubEvent.receiveNotification, $scope.fnOnReceiveNotification);
                notificationHubConnection.start().then(function() {
                    console.log('Notification hub connection has been initialized');
                });
            };

            /*
            * Callback function which is raised when user connects to a private channel.
            * */
            $scope.fnAuthorizeRealTimeConnection = function (channel, options) {
                return {
                    authorize: function (socketId, callback) {
                        return pusherService.authorizeRealTimeChannel(channel.name, socketId)
                            .then(function (channelAuthenticationResponse) {
                                if (!channelAuthenticationResponse)
                                    return;

                                var channelAuthenticationResult = channelAuthenticationResponse.data;
                                if (!channelAuthenticationResult)
                                    return;

                                var auth = channelAuthenticationResult.auth;
                                callback(false, auth);
                            });
                    }
                }
            };

            //#endregion

            //#region Hub events

            /*
            * Function which is raised when notification which sent from service is received.
            * */
            $scope.fnOnReceiveNotification = function(notification){
                var notificationCategory = notification.category;
                var notificationAction = notification.action;
                var data = notification.data;

                // Construct a message to display to user.
                var szMessage = '';

                switch (notificationAction){
                    case notificationActionConstant.add:
                        szMessage = $translate.instant('User created a category', {username: data.creator, category: data.category});
                        break;

                    case notificationAction.update:
                        szMessage = $translate.instant('User updated a category', {username: data.creator, category: data.category});
                        break;

                    case notificationAction.delete:
                        szMessage = $translate.instant('User deleted a category', {username: data.creator, category: data.category});
                        break;
                }

                if (szMessage)
                    toastr.info(szMessage);
            };

            //#endregion
        });
};