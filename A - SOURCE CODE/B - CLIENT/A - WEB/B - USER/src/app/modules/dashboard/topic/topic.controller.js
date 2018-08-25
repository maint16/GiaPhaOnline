module.exports = (ngModule) => {

    // Import constant files.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

    ngModule.controller('topicController', ($categoryGroup,
                                            $state,
                                            $scope) => {

        //#region Properties

        // Topic information.
        $scope._topic = {};

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