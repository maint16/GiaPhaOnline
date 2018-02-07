module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./post-search.html');

    // Directive declaration.
    ngModule.directive('postSearch', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                ngClickCancel: '&',
            },
            controller: function($scope, urlStates){

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                // Model which is for information binding.
                $scope.model = {
                    title: null
                };
                //#endregion

                //#region Methods

                /*
                * Event which is fired when cancel button is clicked.
                * */
                $scope.cancel = function(){
                    $scope.ngClickCancel();
                };

                //#endregion
            }
        }
    });
};