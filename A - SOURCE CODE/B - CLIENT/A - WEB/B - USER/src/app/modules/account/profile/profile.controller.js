module.exports = function(ngModule){

    /*
    * Initialize controller with injectors.
    * */
    ngModule.controller('profileController', function(profile, personalProfile,
                                                      $ngConfirm, $uibModal, $translate, $state,
                                                      userService, authenticationService,
                                                      $scope){

        //#region Properties

        // Resolver reflection.
        $scope.personalProfile = personalProfile;

        // Modal dialog instances.
        $scope.modals = {
            changePassword: null
        };

        // Model for data binding.
        $scope.model = {
            changePassword: {
                currentPassword: null,
                password: null,
                confirmPassword: null
            }
        };

        //#endregion

        //#region Methods

        /*
        * Event which is fired when change password button is clicked.
        * */
        $scope.fnClickChangePassword = function(){

            // Not the profile owner.
            if (!profile || profile.id !== $scope.personalProfile.id)
                return;

            // Modal is valid. Close it first.
            if ($scope.modals.changePassword){
                $scope.modals.changePassword.close();
                $scope.modals.changePassword = null;
            }

            $scope.modals.changePassword = $uibModal.open({
                templateUrl: 'change-password.html',
                scope: $scope,
                size: 'lg'
            });
        };

        /*
        * Event which is for submitting new password.
        * */
        $scope.fnSubmitPassword = function($event, bIsFormValid){

            // Prevent default behaviour.
            if ($event)
                $event.preventDefault();

            // Form is invalid.
            if (!bIsFormValid)
                return;

            // Submit information to server.
            userService.changePassword($scope.personalProfile.id, $scope.model.changePassword)
                .then(function(changePasswordResponse){

                    // Close modal dialog.
                    if ($scope.modals.changePassword){
                        $scope.modals.changePassword.dismiss();
                        $scope.modals.changePassword = null;
                    }

                    // Get result.
                    var changePasswordResult = changePasswordResponse.data;
                    if (!changePasswordResult){
                        return;
                    }
                    // As user is changing his/her own password. Obtain the access token returned by this api.
                    if (profile.id === $scope.personalProfile.id){
                        var szAccessToken = changePasswordResult.accessToken;
                        authenticationService.initAuthenticationToken(szAccessToken);
                    }

                    // Reload the current state.
                    $state.reload();
                });
        };

        //#endregion
    });
};