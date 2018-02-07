/*
* Module exports.
* */
module.exports = function(ngModule){
    ngModule.controller('mainDashboardController', function($scope, toastr){

        //#region Properties

        $scope.result = {
            getCategories:{
                categories: [],
                total: 100
            }
        };

        //#endregion

    });
};