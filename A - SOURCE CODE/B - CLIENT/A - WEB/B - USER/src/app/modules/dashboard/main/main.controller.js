/*
* Module exports.
* */
module.exports = function(ngModule){
    ngModule.controller('mainDashboardController', function($scope, toastr,
                                                            $timeout, $state,
                                                            initialGetCategory,
                                                            urlStates){

        //#region Properties

        // Search result caching.
        $scope.result = {
            getCategories:{
                records: [],
                total: 0
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
        * Called when controller has been initialized successfully.
        * */
        $timeout(function(){
            $scope.result.getCategories = initialGetCategory;
        });

        //#endregion

    });
};