module.exports = function (ngModule) {
    ngModule.controller('authorizedLayoutController',
        function ($scope, $state, $transitions, uiService,
                  profile, $uibModal, $timeout, $window,
                  authenticationService) {

            //#region Properties

            // Resolver reflection.
            $scope.profile = profile;

            // Modal dialogs list.
            $scope.modals = {
                login: null
            };

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
            $scope.fnClickBasicLogin = function () {
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
            $scope.fnLoginSuccessfully = function (token) {

                // Save access token into storage.
                authenticationService.initAuthenticationToken(token.accessToken);

                // Dismiss the modal.
                if ($scope.modals.login) {
                    $scope.modals.login.dismiss();
                    $scope.modals.login = null;
                }

                // Reload the state.
                $state.reload();
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
                // var apiGoogleScript = document.createElement('script');
                // apiGoogleScript.type = 'text/javascript';
                // apiGoogleScript.async = true;
                // apiGoogleScript.src = 'https://apis.google.com/js/client.js?onload=fnSignInGoogle';
                // var scriptTag = document.getElementByName('script')[0];
                // scriptTag.parentNode.insertBefore(apiGoogleScript, scriptTag);

                var apiGoogleScript = document.createElement('script');
                apiGoogleScript.type = 'text/javascript';
                apiGoogleScript.async = true;
                apiGoogleScript.src = 'https://apis.google.com/js/platform.js?onload=fnGoogleClientInitialized';
                document.head.appendChild(apiGoogleScript);

                $window.fnGoogleClientInitialized = function () {

                    gapi.load('auth2', function () {
                        console.log('Auth2 ready');

                        var params = {
                            client_id: '323676358406-ikvol20relacv3mn5popdi79e5m759pc.apps.googleusercontent.com',
                            scope: 'email',
                            fetch_basic_profile: true
                        };

                        gapi.auth2.init(params);
                    });

                };
            });

            $scope.fnClickGoogleLogin = function () {
                var pGoogleAuthenticationClient = gapi.auth2.getAuthInstance();
                pGoogleAuthenticationClient
                    .grantOfflineAccess({
                        scope: 'profile email'
                    })
                    .then(function (getGoogleCredentialResult) {
                        console.log(getGoogleCredentialResult);
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