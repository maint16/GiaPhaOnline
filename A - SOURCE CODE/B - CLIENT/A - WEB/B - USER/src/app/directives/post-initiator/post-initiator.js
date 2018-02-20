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
            controller: function($scope, urlStates){

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                // Model which is for information binding.
                $scope.model = {
                    title: null,
                    content: null
                };

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

                //#endregion
            }
        }
    });
};