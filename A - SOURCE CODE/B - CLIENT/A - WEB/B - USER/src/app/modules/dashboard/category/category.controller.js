module.exports = (ngModule) => {

    // Import constants.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

    ngModule
        .controller('categoryTopicsController', (
            $topic,
            $state,
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

            //#endregion
        });
};