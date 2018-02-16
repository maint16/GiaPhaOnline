module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./post-initiator.html');

    // Directive declaration.
    ngModule.directive('postInitiator', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                ngClickCancel: '&'
            },
            controller: function($scope, urlStates){

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                $scope.ckEditorOptions = {
                    language: 'en',
                    allowedContent: true,
                    entities: false
                };

                // Model which is for information binding.
                $scope.model = {
                    title: null,
                    content: ''
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