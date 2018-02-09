module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./post-detail-box.html');

    // Directive declaration.
    ngModule.directive('postDetailBox', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                post: '='
            },
            controller: function($scope, urlStates){

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                //#endregion
            }
        }
    });
};