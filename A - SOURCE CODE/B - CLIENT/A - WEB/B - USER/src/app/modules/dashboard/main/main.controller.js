/*
* Module exports.
* */
module.exports = (ngModule) => {

    // Import constants.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

    ngModule.controller('mainDashboardController', ($categoryGroup,
                                                    $scope) => {
        //#region Methods

        // Load category groups result.
        $scope._loadCategoryGroupsResult = {};

        // Constants reflection.
        $scope.urlStateConstants = UrlStateConstant;

        //#endregion

        //#region Events

        /*
        * Called when component is initialized.
        * */
        $scope._ngOnInit = () => {

            $categoryGroup.getCategoryGroups()
                .then((loadCategoriesResult) => {
                    $scope._loadCategoryGroupsResult = loadCategoriesResult;
                })

        };

        //#endregion
    });
};