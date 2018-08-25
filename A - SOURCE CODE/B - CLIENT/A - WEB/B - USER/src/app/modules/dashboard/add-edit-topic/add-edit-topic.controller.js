module.exports = (ngModule) => {
    ngModule.controller('addEditTopicController', ($scope) => {

        //#region Properties

        $scope.oModel = {
            content: null
        };

        /*
        * Editor configuration.
        * */
        $scope._editorConfiguration = {
            resize: false
        }

        //#endregion

    });
};