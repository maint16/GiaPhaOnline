/*
* Module exports.
* */
module.exports = (ngModule) => {

    // Import constants.
    const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;
    const {LoadCategoryViewModel} = require('../../../view-models/load-category.view-model');

    ngModule.controller('mainDashboardController', ($ui,
                                                    $categoryGroup, $category,
                                                    $scope) => {
        //#region Methods

        // Load topics groups result.
        $scope._loadCategoryGroupsResult = {};

        // Mapping between topics group - categories.
        $scope._mCategoryGroupToCategoriesMap = {};

        // Constants reflection.
        $scope.urlStateConstants = UrlStateConstant;

        //#endregion

        //#region Events

        /*
        * Called when component is initialized.
        * */
        $scope._ngOnInit = () => {

            // Build topics group search condition.
            let loadCategoryGroupCondition = {};

            let loadCategoryGroupPromise = $categoryGroup
                .loadCategoryGroups(loadCategoryGroupCondition);

            // Result of loading categories.
            let oLoadCategoryGroupsResult = {
                records: [],
                total: 0
            };

            // Block app ui.
            $ui.blockAppUI();

            loadCategoryGroupPromise
                .then((loadCategoryGroupsResult) => {

                    // Get list of categories group.
                    let categoryGroups = loadCategoryGroupsResult.records;


                    // Get the list of categories ids.
                    let loadCategoriesCondition = new LoadCategoryViewModel();
                    loadCategoriesCondition.pagination = null;
                    loadCategoriesCondition.categoryGroupIds = categoryGroups.map((categoryGroup) => categoryGroup.id);

                    // Update search result.
                    oLoadCategoryGroupsResult = loadCategoryGroupsResult;

                    return $category
                        .loadCategories(loadCategoriesCondition);
                })
                .then((loadCategoriesResult) => {

                    // Get list of categories.
                    let categories = loadCategoriesResult.records;
                    let mCategoryGroupToCategoriesMap = {};

                    for (let category of categories){
                        let categoryGroupId = category.categoryGroupId;
                        if (!mCategoryGroupToCategoriesMap[categoryGroupId])
                            mCategoryGroupToCategoriesMap[categoryGroupId] = [];

                        mCategoryGroupToCategoriesMap[categoryGroupId].push(category);
                    }

                    $scope._mCategoryGroupToCategoriesMap = mCategoryGroupToCategoriesMap;
                    $scope._loadCategoryGroupsResult = oLoadCategoryGroupsResult;
                })
                .finally(() => {
                    $ui.unblockAppUI();
                });
        };

        /*
        * Get categories from group by using group id.
        * */
        $scope.loadCategoriesInCategoryGroup = (iCategoryGroupId) => {
            let mCategoryGroupToCategoriesMap = $scope._mCategoryGroupToCategoriesMap;
            if (!mCategoryGroupToCategoriesMap)
                return [];

            return mCategoryGroupToCategoriesMap[iCategoryGroupId];
        };

        /*
        * Get the list of loaded topics groups which are stored in cache.
        * */
        $scope.loadCategoryGroupsInCache = () => {

            let loadCategoryGroupsResult = $scope._loadCategoryGroupsResult;
            if (!loadCategoryGroupsResult)
                return [];

            return loadCategoryGroupsResult.records;
        }

        //#endregion
    });
};