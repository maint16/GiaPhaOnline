/*
* Module exports.
* */
module.exports = function(ngModule){
    ngModule.controller('mainDashboardController', function($scope, toastr,
                                                            $timeout, $state,
                                                            details,
                                                            appSettings,
                                                            urlStates,
                                                            categoryService, postCategorizationService){

        //#region Properties

        // Search result caching.
        $scope.result = {
            getCategories:{
                records: [],
                total: 0
            }
        };

        // Temporary storage for client caching.
        $scope.buffer = {
            categoryPostsCounter: {}
        };

        // Search condition.
        $scope.condition = {
            getCategories:{
                pagination:{
                    page: 1,
                    records: appSettings.pagination.default
                }
            }
        };

        //#endregion

        //#region Methods

        /*
        * Called when a category is selected.
        * */
        $scope.clickCategory = function(category){
            $state.go(urlStates.category.postListing.name, {categoryId: category.id});
        };

        /*
        * Get categories can concatenate the search result list,
        * */
        $scope.fnGetCategories = function($event){

            // Event is valid. Prevent its default behaviour.
            if ($event)
                $event.preventDefault();

            // Reset page to 1.
            $scope.condition.getCategories.pagination.page = 1;

            // Do searching.
            categoryService.getCategories($scope.condition.getCategories)
                .then(function(getCategoriesResponse){
                    var getCategoriesResult = getCategoriesResponse.data;
                    if (!getCategoriesResult) {
                        // Reset search result.
                        $scope.result.getCategories = {};
                        throw 'Cannot get categories list';
                    }

                    $scope.result.getCategories = getCategoriesResult;
                    return getCategoriesResult;
                })
                .then(function(getCategoriesResult){

                    var categories = getCategoriesResult.records;
                    if (!categories || categories.length < 1)
                        throw 'No category has been found';

                    // Get all categories posts.
                    var getPostCategorizationPromises = [];

                    // Clear buffer.
                    $scope.buffer.categoryPostsCounter = {};

                    angular.forEach(categories, function(category, iterator){

                        // Build condition for getting post categorizations.
                        var getPostCategorizationCondition = {
                            categoryId: category.id,
                            pagination:{
                                page: 1,
                                records: 1
                            }
                        };

                        // Build promise.
                        var getPostCategorizationPromise = postCategorizationService.getPostCategorizations(getPostCategorizationCondition)
                            .then(function(getPostCategorizationResponse){

                                // Get api returned result.
                                var getPostCategorizationResult = getPostCategorizationResponse.data;
                                if (!getPostCategorizationResult)
                                    return;

                                // Get the first result.
                                var categorizations = getPostCategorizationResult.records;
                                if (!categorizations || categorizations.length < 1)
                                    return;

                                var categorization = categorizations[0];
                                if (!categorization)
                                    return;

                                $scope.buffer.categoryPostsCounter[categorization.categoryId] = getPostCategorizationResult.total;
                            });

                        // Add promise to queue.
                        getPostCategorizationPromises.push(getPostCategorizationPromise);
                    });

                    return Promise.all(getPostCategorizationPromises).then(function(x){
                        console.log($scope.buffer.categoryPostsCounter);
                    });
                });
        };

        /*
        * Called when controller has been initialized successfully.
        * */
        $timeout(function(){
            $scope.result.getCategories.records = details.categories;
            $scope.buffer.categoryPostsCounter = details.categoryPostsCounter;
            // $scope.result.getCategories = initialGetCategory;
        });

        //#endregion

    });
};