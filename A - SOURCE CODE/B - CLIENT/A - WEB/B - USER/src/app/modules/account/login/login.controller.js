module.exports = (ngModule) => {

    // Import constants.
    const UrlStateConstant = require('../../../constants/url-state.constant').UrlStateConstant;

    ngModule.controller('loginController', (
        $state,
        $scope) => {

        //#region Properties

        // Constant reflection.
        $scope.urlStateConstant = UrlStateConstant;

        //#endregion

        //#region Methods

        /*
        * Called when login button is clicked.
        * */
        $scope.clickLogin = () => {
            $state.go(UrlStateConstant.dashboardModuleName);
        };

        //#endregion
    });
};