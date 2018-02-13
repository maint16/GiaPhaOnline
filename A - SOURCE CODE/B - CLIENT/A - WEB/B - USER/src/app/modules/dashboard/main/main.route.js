module.exports = function (ngModule) {

    // Module html template import.
    var ngModuleHtmlTemplate = require('./main.html');

    ngModule.config(function ($stateProvider, urlStates) {

        var urlStateDashboard = urlStates.dashboard;
        var urlStateAuthorizedLayout = urlStates.authorizedLayout;

        $stateProvider.state(urlStateDashboard.name, {
            url: urlStateDashboard.url,
            controller: 'mainDashboardController',
            parent: urlStateAuthorizedLayout.name,
            template: ngModuleHtmlTemplate,
            resolve: {

                // List of default categories.
                details: function (categoryService, postCategorizationService, appSettings) {

                    // Information which should be returned from promise.
                    var details = {
                        categories: null,
                        categoryPostsCounter: {}
                    };

                    var getCategoriesCondition = {
                        pagination: {
                            page: 1,
                            records: appSettings.pagination.default
                        }
                    };

                    // Get list of categories.
                    return categoryService.getCategories(getCategoriesCondition)
                        .then(function(getCategoriesResponse){

                            var getCategoriesResult = getCategoriesResponse.data;
                            if (!getCategoriesResult)
                                return details;

                            var categories = getCategoriesResult.records;
                            details.categories = categories;

                            if (!categories || categories.length < 1)
                                return details;

                            // Build promises list.
                            var getPostCategorizationPromises = [];

                            // Go through every category and get its posts counter.
                            angular.forEach(categories, function(category, iterator){

                                var getPostCategorizationCondition = {
                                    categoryId: category.id,
                                    pagination:{
                                        page: 1,
                                        records: 1
                                    }
                                };

                                // Already in buffer.
                                if (details.categoryPostsCounter[category.id])
                                    return;

                                var getPostCategorizationPromise = postCategorizationService.getPostCategorizations(getPostCategorizationCondition)
                                    .then(function(getPostCategorizationResponse){
                                        var getPostCategorizationResult = getPostCategorizationResponse.data;
                                        if (!getPostCategorizationResult)
                                            return null;

                                        var postCategorizations = getPostCategorizationResult.records;
                                        if (!postCategorizations || postCategorizations.length < 1)
                                            return null;

                                        // Get the first categorization.
                                        var postCategorization = postCategorizations[0];
                                        if (!postCategorization)
                                            return null;

                                        details.categoryPostsCounter[postCategorization.categoryId] = getPostCategorizationResult.total;
                                    });

                                // Add promise to list.
                                getPostCategorizationPromises.push(getPostCategorizationPromise);
                            });

                            return Promise.all(getPostCategorizationPromises).then(function(){
                                return details;
                            });
                        });
                }
            }
        });
    });
};