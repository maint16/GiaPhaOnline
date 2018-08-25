module.exports = (ngModule) => {

    // Import constants.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

    ngModule.controller('forgotPasswordController', (
        $state,
        $scope) => {

        //#region Properties

        $scope.urlStateConstant = UrlStateConstant;

        //#endregion

        //#region Methods

        /*
        * Called when send password reset button is clicked.
        * */
        $scope.clickSendPasswordResetEmail = () => {

        }

        //#endregion
    });
};