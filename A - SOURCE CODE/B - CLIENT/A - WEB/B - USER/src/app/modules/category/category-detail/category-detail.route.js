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
                details: function ($state, $stateParams,
                                   urlStates,
                                   profile,
                                   followCategoryService, categoryService) {


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


                    var getCategoryCondition = {
                        id: iCategoryId,
                        pagination: {
                            page: 1,
                            records: 1
                        }
                    };

                    // Information which should be returned from promise.
                    var details = {
                        bIsFollowingCategory: false,
                        category: null
                    };

                    // Get category information first.
                    return categoryService.getCategories(getCategoryCondition)
                        .then(function(getCategoriesResponse){

                            // Get result from api.
                            var getCategoriesResult = getCategoriesResponse.data;

                            // Invalid response.
                            if (!getCategoriesResult){
                                $state.go(urlStates.dashboard.name);
                                throw 'Unable to get category information';
                            }

                            var categories = getCategoriesResult.records;
                            if (!categories || categories.length < 1){
                                $state.go(urlStates.dashboard.name);
                                throw 'Unable to get category information';
                            }

                            // Update detail information.
                            details.category = categories[0];
                            return categories[0];
                        })

                        // Get following category information.
                        .then(function(category){

                            // User is anonymous. Skip this step.
                            if (profile == null)
                                return details;

                            // Get following categories conditions.
                            var getFollowingCategoriesCondition = {
                                categoryId: category.id,
                                pagination: {
                                    page: 1,
                                    records: 1
                                }
                            };

                            return followCategoryService.getFollowingCategories(getFollowingCategoriesCondition)
                                .then(function (getFollowingCategoriesResponse) {
                                    var getFollowingCategoriesResult = getFollowingCategoriesResponse.data;
                                    if (!getFollowingCategoriesResult)
                                        return details;

                                    var followingCategories = getFollowingCategoriesResult.records;
                                    if (!followingCategories || followingCategories.length < 1)
                                        return details;

                                    details.bIsFollowingCategory = true;
                                    return details
                                });
                        });

                }

            }
        });
    });
};