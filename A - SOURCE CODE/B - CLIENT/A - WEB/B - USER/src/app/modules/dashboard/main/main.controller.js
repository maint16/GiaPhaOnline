/*
* Module exports.
* */
module.exports = function (ngModule) {
    ngModule.controller('mainDashboardController', function ($scope, toastr, $ngConfirm, $translate,
                                                             $timeout, $state,
                                                             appSettings, urlStates,
                                                             profile,
                                                             categoryService, postCategorizationService, followCategoryService) {

        //#region Properties

        // Resolver reflection.
        $scope.profile = profile;

        // Search result caching.
        $scope.result = {
            getCategories: {
                records: [],
                total: 0
            }
        };

        // Temporary storage for client caching.
        $scope.buffer = {
            categoryPostsCounter: {},
            followingCategory: {}
        };

        // Search condition.
        $scope.condition = {
            getCategories: {
                pagination: {
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
        $scope.clickCategory = function (category) {
            $state.go(urlStates.category.postListing.name, {categoryId: category.id});
        };

        /*
        * Function for searching categories (start searching from the beginning)
        * */
        $scope.fnSearchCategories = function($event){
            // Event is valid. Prevent its default behaviour.
            if ($event)
                $event.preventDefault();

            // Reset pagination to page 1.
            $scope.condition.getCategories.pagination.page = 1;

            // Search for categories.
            $scope.fnGetCategories();
        };

        /*
        * Get categories can concatenate the search result list,
        * */
        $scope.fnGetCategories = function () {

            // Clear buffer.
            $scope.buffer.followingCategory = {};
            $scope.buffer.categoryPostsCounter = {};
            $scope.result.records = [];
            $scope.result.total = 0;

            // Do searching.
            categoryService.getCategories($scope.condition.getCategories)
                .then(function (getCategoriesResponse) {
                    var getCategoriesResult = getCategoriesResponse.data;
                    if (!getCategoriesResult) {
                        // Reset search result.
                        $scope.result.getCategories = {};
                        throw 'Cannot get categories list';
                    }

                    return getCategoriesResult;
                })
                .then(function (getCategoriesResult) {

                    var categories = getCategoriesResult.records;
                    if (!categories || categories.length < 1)
                        throw 'No category has been found';

                    // Get all categories posts.
                    var pPromises = [];

                    // Clear buffer.
                    $scope.buffer.categoryPostsCounter = {};

                    angular.forEach(categories, function (category, iterator) {

                        //#region Get category posts counter

                        // Build condition for getting post categorizations.
                        var getPostCategorizationCondition = {
                            categoryId: category.id,
                            pagination: {
                                page: 1,
                                records: 1
                            }
                        };

                        // Get category post counter promise.
                        var pGetCategoryPostCounterPromise = postCategorizationService.getPostCategorizations(getPostCategorizationCondition)
                            .then(function (getPostCategorizationResponse) {

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
                        pPromises.push(pGetCategoryPostCounterPromise);

                        //#endregion

                        //#region Get category following status

                        // If user is authenticated. Search for his/her following category.
                        if (profile) {
                            // Build condition for getting category following status
                            var getCategoryFollowingStatus = {
                                categoryId: category.id,
                                pagination: {
                                    page: 1,
                                    records: 1
                                }
                            };

                            // Build get category following status promise.
                            var pGetCategoryFollowingStatusPromise = followCategoryService.getFollowingCategories(getCategoryFollowingStatus)
                                .then(function (getCategoryFollowingStatusResponse) {

                                    // Get following status result.
                                    var getCategoryFollowingStatusResult = getCategoryFollowingStatusResponse.data;
                                    if (getCategoryFollowingStatusResult == null)
                                        return;

                                    // No record is found.
                                    var records = getCategoryFollowingStatusResult.records;
                                    if (records == null || records.length < 1)
                                        return;

                                    var record = records[0];
                                    if (record == null)
                                        return;

                                    $scope.buffer.followingCategory[record.categoryId] = true;


                                });

                            // Add promise to list.
                            pPromises.push(pGetCategoryFollowingStatusPromise);
                        }

                        //#endregion
                    });

                    return Promise.all(pPromises)
                        .then(function (x) {
                            return getCategoriesResult;
                        });
                })
                .then(function(getCategoriesResult){
                    $scope.result.getCategories = getCategoriesResult;
                });
        };

        /*
        * Unfollow the specific category.
        * */
        $scope.fnUnfollowCategory = function(categoryId){
            // Category is not being followed yet.
            if (!$scope.buffer.followingCategory[categoryId])
                return;

            // Display confirmation message.
            $ngConfirm({
                title: ' ',
                content: '<b class="text-danger">{{"Stop following this category" | translate}}</b>',
                scope: $scope,
                buttons: {
                    ok: {
                        text: $translate.instant('OK'),
                        btnClass: 'btn btn-flat btn-danger',
                        action: function(scope, button){
                            // Unfollow category.
                            followCategoryService.unfollowCategory(categoryId)
                                .then(function(){
                                    var szMessage = $translate.instant('Stopped following category successfully');
                                    toastr.success(szMessage);

                                    // Mark the category be unfollowed.
                                    $scope.buffer.followingCategory[categoryId] = false;
                                });
                        }
                    },
                    cancel: {
                        text: $translate.instant('Cancel'),
                        btnClass: 'btn btn-flat btn-default',
                        action: function(scope, button){
                        }
                    }
                }
            });
        };

        /*
        * Follow the specific category.
        * */
        $scope.fnFollowCategory = function(categoryId){
            // Category has been followed.
            if ($scope.buffer.followingCategory[categoryId])
                return;

            // Display confirmation message.
            $ngConfirm({
                title: ' ',
                content: '<b class="text-info">{{"Start following this category" | translate}}</b>',
                scope: $scope,
                buttons: {
                    ok: {
                        text: $translate.instant('OK'),
                        btnClass: 'btn btn-flat btn-primary',
                        action: function(scope, button){
                            // Unfollow category.
                            followCategoryService.followCategory(categoryId)
                                .then(function(){
                                    var szMessage = $translate.instant('Followed category successfully');
                                    toastr.success(szMessage);

                                    // Mark the category be unfollowed.
                                    $scope.buffer.followingCategory[categoryId] = true;
                                });
                        }
                    },
                    cancel: {
                        text: $translate.instant('Cancel'),
                        btnClass: 'btn btn-flat btn-default',
                        action: function(scope, button){
                        }
                    }
                }
            });
        };

        /*
        * Called when controller has been initialized successfully.
        * */
        $scope.fnInit = function () {
            // Search for categories.
            $scope.condition.getCategories.pagination.page = 1;
            $scope.fnGetCategories();
        };

        //#endregion

    });
};