module.exports = (ngModule) => {

    // Import constants.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

    ngModule
        .controller('categoryTopicsController', (
            $state,
            $topic,
            $scope) => {

            //#region Properties

            // Constants reflection.
            $scope.urlStateConstant = UrlStateConstant;

            // Load categories result.
            $scope._loadTopicsResult = {};

            //#endregion
            
            //#region Methods
            
            //#endregion
            
            //#region Events

            /*
            * Called when component is initialized.
            * */
            $scope._ngOnInit = () => {
                $topic.loadTopics()
                    .then((loadTopicsResult) => {
                        $scope._loadTopicsResult = loadTopicsResult;
                    });
            };

            /*
            * Call when add topic button is clicked.
            * */
            $scope._ngOnAddTopicClicked = () => {
                $state.go(UrlStateConstant.addTopicModuleName);
            }

            //#endregion
        });
};