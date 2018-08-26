module.exports = (ngModule) => {

    // Import constants.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

    ngModule.controller('addEditTopicController', (
        $state,
        $scope) => {

        //#region Properties

        // Topic information.
        $scope._topic = {
            id: 0,
            title: null,
            content: '<b>Hello world</b>'
        };

        // Whether editor has been initialized or not.
        $scope.bIsEditorInitialized = false;

        // Whether preview mode is on or off.
        $scope.bIsInPreviewMode = false;

        /*
        * Editor configuration.
        * */
        $scope._editorConfiguration = {
            resize: false,
            height: 500,
            setup: (editor) => {
                editor.on("init", () => {
                    $scope.bIsEditorInitialized = true;
                });
            }
        };

        //#endregion

        //#region Events

        /*
        * Called when preview toggle button is clicked.
        * */
        $scope._ngOnPreviewToggleClick = () => {
            $scope.bIsInPreviewMode = !$scope.bIsInPreviewMode;
        };

        /*
        * Called when ok button is clicked.
        * */
        $scope._ngOnOkClicked = () => {
            $state.go(UrlStateConstant.topicModuleName);
        };

        /*
        * Called when editor is cancelled editing.
        * */
        $scope._ngOnCancelEditorClicked = () => {
            // Redirect user to topics list.
            $state.go(UrlStateConstant.topicModuleName);
        }

        //#endregion
    });
};