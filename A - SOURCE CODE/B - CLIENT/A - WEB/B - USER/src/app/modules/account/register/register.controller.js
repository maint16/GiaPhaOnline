module.exports = (ngModule) => {

    // Import constants.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

    // Controller initialization.
    ngModule.controller('accountRegistrationController', (
        $state,
        $scope) => {

        //#region Properties

        // Constants reflection.
        $scope.urlStateConstant = UrlStateConstant;

        //#endregion

        //#region Methods


        //#endregion

        //#region Events

        /*
        * Called when user is registered.
        * */
        $scope.ngOnUserRegister = () => {

            // Redirect user login page.
            $state.go(UrlStateConstant.loginModuleName);
        }

        //#endregion
    });
};