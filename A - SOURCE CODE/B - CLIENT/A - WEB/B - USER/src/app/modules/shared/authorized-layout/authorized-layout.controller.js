module.exports = function (ngModule) {
    ngModule.controller('authorizedLayoutController',
        function ($scope, $state, $transitions, uiService,
                  profile, $uibModal,
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
            $scope.fnClickBasicLogin = function(){
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
            $scope.fnLoginSuccessfully = function(token){

                // Save access token into storage.
                authenticationService.initAuthenticationToken(token.accessToken);

                // Dismiss the modal.
                if ($scope.modals.login){
                    $scope.modals.login.dismiss();
                    $scope.modals.login = null;
                }

                // Reload the state.
                $state.reload();
            };

            /*
            * Event which is fired when sign out button is clicked.
            * */
            $scope.fnSignOut = function(){
                // Clear access token.
                authenticationService.clearAuthenticationToken();

                // Reload the current state.
                $state.reload();
            };

            /*
            * Event which is fired when cancel button is clicked.
            * */
            $scope.fnCancelLogin = function(){
                if (!$scope.modals.login){
                    return;
                }

                $scope.modals.login.dismiss();
                $scope.modals.login = null;
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