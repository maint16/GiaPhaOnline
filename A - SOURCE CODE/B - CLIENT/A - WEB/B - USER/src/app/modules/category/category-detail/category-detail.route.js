module.exports = function (ngModule) {

    // Module html template import.
    var ngModuleHtmlTemplate = require('./category-detail.html');

    ngModule.config(function ($stateProvider, urlStates) {

        var urlStateCategoryDetail = urlStates.category.postListing;
        var urlStateAuthorizedLayout = urlStates.authorizedLayout;

        $stateProvider.state(urlStateCategoryDetail.name, {
            url: urlStateCategoryDetail.url,
            controller: 'categoryDetailController',
            parent: urlStateAuthorizedLayout.name,
            template: ngModuleHtmlTemplate,
            resolve: {

                // Follow category information.
                followCategory: function ($state, $stateParams,
                                            urlStates,
                                            profile, followCategoryService) {
                    if (profile == null)
                        return null;

                    var szCategoryId = $stateParams.categoryId;
                    if (!szCategoryId) {
                        $state.go(urlStates.dashboard.name);
                        return;
                    }

                    // Get category id.
                    var iCategoryId = parseInt(szCategoryId);
                    if (!iCategoryId) {
                        $state.go(urlStates.dashboard.name);
                        return;
                    }

                    var conditions = {
                        categoryId: iCategoryId,
                        pagination:{
                            page: 1,
                            records: 1
                        }
                    };

                    return followCategoryService.getFollowingCategories(conditions)
                        .then(function(getFollowingCategoriesResponse){
                            var getFollowingCategoriesResult = getFollowingCategoriesResponse.data;
                            if (!getFollowingCategoriesResult)
                                return null;

                            var followingCategories = getFollowingCategoriesResult.records;
                            return followingCategories[0];
                        });
                }
            }
        });
    });
};