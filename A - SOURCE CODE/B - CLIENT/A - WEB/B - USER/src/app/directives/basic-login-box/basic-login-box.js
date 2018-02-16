module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./basic-login-box.html');

    // Directive declaration.
    ngModule.directive('basicLoginBox', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                ngCancel: '&',
                ngLoginSuccessfully: '&',
                ngLoginFailingly: '&'
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

                    userService.basicLogin($scope.model)
                        .then(
                            function success(basicLoginResponse) {
                                var basicLoginResult = basicLoginResponse.data;
                                $scope.ngLoginSuccessfully({token: basicLoginResult});
                            },
                            function error(basicLoginResponse) {
                                $scope.ngLoginFailingly();
                            });
                };

                /*
                * Event which is fired when cancel button is clicked.
                * */
                $scope.fnCancel = function(){
                    $scope.ngCancel();
                };

                //#endregion
            }
        }
    });
};