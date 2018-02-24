module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./post-initiator.html');

    // Directive declaration.
    ngModule.directive('postInitiator', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                ngClickCreatePost: '&',
                ngClickCancel: '&'
            },
            controller: function($scope, urlStates, postTypeConstant,
                                 appSettings,
                                 categoryService){

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                // Model which is for information binding.
                $scope.model = {
                    title: null,
                    content: null,
                    type: postTypeConstant.public,
                    categories: null
                };

                // Cache information.
                $scope.cache = {
                    categories: null
                };

                // Type of post.
                $scope.postTypes = [
                    {
                        id: postTypeConstant.public,
                        name: 'Public'
                    },
                    {
                        id: postTypeConstant.private,
                        name: 'Private'
                    }
                ];

                // Editor option.
                $scope.editorOptions = {
                    disableResizeEditor: true
                };
                //#endregion

                //#region Methods

                /*
                * Event which is fired when cancel button is clicked.
                * */
                $scope.cancel = function(){
                    $scope.ngClickCancel();
                };

                /*
                * Callback which is fired when component is initiated.
                * */
                $scope.fnInit = function(){
                    // Search for categories.
                    $scope.fnGetCategories(null);
                };

                /*
                * Event which is fired when post is created.
                * */
                $scope.fnClickCreatePost = function($event){
                    // Event is valid. Prevent its default behaviour.
                    if ($event)
                        $event.preventDefault();

                    // Form is invalid.
                    if (!$scope.postInitiatorForm.$invalid)
                        return;

                    $scope.ngCreatePost({post: $scope.model});
                };

                /*
                * Event which is fired when multi selector control searches for a category.
                * */
                $scope.fnGetCategories = function(keyword){

                    // Build query condition.
                    var getCategoriesCondition = {
                        name: keyword,
                        pagination:{
                            page: 1,
                            records: appSettings.pagination.categoriesSelector
                        }
                    };

                    categoryService.getCategories(getCategoriesCondition)
                        .then(function(getCategoriesResponse){

                            // Get the result.
                            var getCategoriesResult = getCategoriesResponse.data;

                            // Result is invalid.
                            if (!getCategoriesResult)
                                return;

                            var categories = getCategoriesResult.records;
                            if (!categories || categories.length < 1)
                                return;

                            $scope.cache.categories = categories;
                        })
                };

                //#endregion
            }
        }
    });
};