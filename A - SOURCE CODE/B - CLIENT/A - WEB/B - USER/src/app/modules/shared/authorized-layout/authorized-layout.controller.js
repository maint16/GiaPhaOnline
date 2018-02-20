module.exports = function (ngModule) {
    ngModule.controller('authorizedLayoutController',
        function (oAuthSettings,
                  $scope, $state, $transitions, uiService, oAuthService,
                  profile, $uibModal, $timeout, $window,
                  authenticationService, userService) {

            //#region Properties

            // Resolver reflection.
            $scope.profile = profile;

            // Modal dialogs list.
            $scope.modals = {
                login: null
            };

            // Whether google login has been loaded or not.
            $scope.bIsGoogleLoginLoaded = false;
            $scope.bIsFacebookLoginLoaded = false;

            //#endregion

            //#region Methods

            /*
            * Callback which is called when component starts being initiated.
            * */
            $scope.init = function () {
                uiService.reloadWindowSize();
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
                            authenticationService.initAuthenticationToken(basicLoginResult.accessToken);

                            // Dismiss the modal.
                            if ($scope.modals.login) {
                                $scope.modals.login.dismiss();
                                $scope.modals.login = null;
                            }

                            // Reload the state.
                            $state.reload();
                        },
                        function error(basicLoginResponse) {
                            $scope.ngLoginFailingly();
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

                // FB.getLoginStatus(function(response) {
                //     console.log(response);
                //     debugger;
                // });

                // Sign user into system.
                FB.login(function(response) {
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
                        .then(function(loginResponse){
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
            * Event which will be raised when layout has been initialized.
            * */
            $timeout(function () {

                // Add google sdk to page.
                oAuthService.addGoogleSdk('fnGoogleClientInitialized');

                // Add facebook sdk to page.
                oAuthService.addFacebookSdk();
            });

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
            $window.fbAsyncInit = function() {

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
            * Hook the transition from state to state.
            * */
            $transitions.onSuccess({}, function (transition) {
                uiService.reloadWindowSize();
            });
            //#endregion
        });
};