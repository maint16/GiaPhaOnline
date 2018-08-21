module.exports = (ngModule) => {
    ngModule.controller('loginController', ($scope, urlStates) => {

        //#region Properties

        // Constants reflection.
        $scope.urlStates = urlStates;

        //#endregion
    });
};