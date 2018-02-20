module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./basic-login-box.html');

    // Directive declaration.
    ngModule.directive('basicLoginBox', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                ngSupportGoogleLogin: '=',
                ngSupportFacebookLogin: '=',
                ngCancel: '&',
                ngClickBasicLogin: '&',
                ngClickGoogleLogin: '&',
                ngClickFacebookLogin: '&'
            },
            controller: function ($scope, urlStates, userService) {

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                // Model for 2-way data binding.
                $scope.model = {
                    email: null,
                    password: null
                };

                //#endregion

                //#region Methods

                /*
                * Login into system.
                * */
                $scope.fnLogin = function ($event) {

                    // Event is valid. Prevent its default behaviour.
                    if ($event)
                        $event.preventDefault();

                    // Form is invalid.
                    if ($scope.basicLoginForm.$invalid)
                        return;

                    // Emit click basic login event.
                    $scope.ngClickBasicLogin({model: $scope.model});
                };

                /*
                * Event which is fired when Google login is clicked.
                * */
                $scope.fnClickGoogleLogin = function () {
                    $scope.ngClickGoogleLogin();
                };

                /*
                * Event which is fired when Facebook login is clicked.
                * */
                $scope.fnClickFacebookLogin = function(){
                    $scope.ngClickFacebookLogin();
                };

                /*
                * Event which is fired when cancel button is clicked.
                * */
                $scope.fnCancel = function () {
                    $scope.ngCancel();
                };

                //#endregion
            }
        }
    });
};