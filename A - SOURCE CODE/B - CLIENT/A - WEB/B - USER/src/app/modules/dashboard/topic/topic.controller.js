module.exports = (ngModule) => {

    // Import constant files.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

    ngModule.controller('topicController', ($categoryGroup,
                                            $state,
                                            $scope) => {

        //#region Properties

        // Topic information.
        $scope._topic = {};

        // Editor model.
        $scope._oModel = {
            content: null
        };

        // Whether editor has been initialized or not.
        $scope.bHasEditorInitialized = false;

        // Editor configuration.
        $scope._editorConfiguration = {
            resize: false,
            height: 300,
            setup: (editor) => {
                editor.on("init", () => {
                    $scope.bHasEditorInitialized = true;
                });
            }
        };

        //#endregion

        //#region Events

        /*
        * Called when component is initialized successfully.
        * */
        $scope._ngOnInit = () => {
            $categoryGroup
                .getCategoryTopics()
                .then((topic) => {
                    $scope._topic = topic;
                });
        };

        /*
        * Called when add topic is clicked.
        * */
        $scope._ngOnAddTopicClicked = () => {
            $state.go(UrlStateConstant.addTopicModuleName);
        };

        //#endregion
    });
};