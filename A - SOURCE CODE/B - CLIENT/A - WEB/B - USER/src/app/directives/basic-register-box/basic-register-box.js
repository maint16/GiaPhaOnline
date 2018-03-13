module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./basic-register-box.html');

    // Directive declaration.
    ngModule.directive('basicRegisterBox', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                ngClickRegister: '&',
                ngClickCancel: '&'
            },
            controller: function ($scope, urlStates, userService) {

                //#region Properties

                // Model for information binding.
                $scope.model = {
                    email: null,
                    password: null,
                    confirmPassword: null,
                    nickname: null
                };

                //#endregion

                //#region Methods

                /*
                * Event which is called when register button is clicked.
                * */
                $scope.fnBasicRegister = function ($event) {

                    // Event is valid. Cancel its default behaviour first.
                    if ($event)
                        $event.preventDefault();

                    // Raise event to external components.
                    $scope.ngClickRegister({user: $scope.model});
                };

                /*
                * Event which is called when cancel button is clicked.
                * */
                $scope.fnCancel = function () {
                    $scope.ngClickCancel();
                };

                //#endregion
            }
        }
    });
};