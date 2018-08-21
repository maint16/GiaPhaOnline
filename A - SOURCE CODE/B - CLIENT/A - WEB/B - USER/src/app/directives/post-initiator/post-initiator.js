module.exports = (ngModule) => {

    // Directive declaration.
    ngModule.directive('postInitiator', () => {
        return {
            compile: () => {
                let pGetTemplatePromise = $q((resolve) => {
                    require.ensure([], () => resolve(require('./post-initiator.html')));
                });

                return (scope, element) => {
                    pGetTemplatePromise
                        .then((htmlTemplate) => {
                            element.html(htmlTemplate);
                            $compile(element.contents())(scope)
                        });
                };
            },
            restrict: 'E',
            scope: {
                ngClickCreatePost: '&',
                ngClickCancel: '&'
            },
            controller: ($scope, urlStates, postTypeConstant,
                         appSettingConstant,
                         categoryService) => {

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                // Model which is for information binding.
                $scope.model = {
                    title: null,
                    body: null,
                    type: postTypeConstant.public,
                    categories: []
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
                $scope.cancel = function () {
                    $scope.ngClickCancel();
                };

                /*
                * Callback which is fired when component is initiated.
                * */
                $scope.fnInit = function () {
                    // Search for categories.
                    $scope.fnGetCategories(null);
                };

                /*
                * Event which is fired when post is created.
                * */
                $scope.fnClickCreatePost = function ($event) {
                    // Event is valid. Prevent its default behaviour.
                    if ($event)
                        $event.preventDefault();

                    // Form is invalid.
                    if ($scope.postInitiatorForm.$invalid)
                        return;

                    var model = angular.copy($scope.model);
                    model.categories = $scope.model.categories.map(function (x) {
                        return x.id;
                    });

                    $scope.ngClickCreatePost({post: model});
                };

                /*
                * Event which is fired when multi selector control searches for a category.
                * */
                $scope.fnGetCategories = function (keyword) {

                    // Build query condition.
                    var getCategoriesCondition = {
                        name: keyword,
                        pagination: {
                            page: 1,
                            records: appSettingConstant.pagination.categoriesSelector
                        }
                    };

                    categoryService.getCategories(getCategoriesCondition)
                        .then(function (getCategoriesResponse) {

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

                /*
                * Event which is raised when close button is clicked.
                * */
                $scope.fnClickCloseBox = function () {
                    $scope.ngClickCancel();
                };

                //#endregion
            }
        }
    });
};