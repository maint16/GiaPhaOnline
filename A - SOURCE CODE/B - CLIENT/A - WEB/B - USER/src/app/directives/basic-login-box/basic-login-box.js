module.exports = (ngModule) => {

    // Directive declaration.
    ngModule.directive('basicLoginBox', () => {
        return {
            compile: () => {
                let pGetTemplatePromise = $q((resolve) => {
                    require.ensure([], () => resolve(require('./basic-login-box.html')));
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
                ngSupportGoogleLogin: '=',
                ngSupportFacebookLogin: '=',
                ngCancel: '&',
                ngClickBasicLogin: '&',
                ngClickGoogleLogin: '&',
                ngClickFacebookLogin: '&',
                ngClickBasicRegister: '&'
            },
            controller: ($scope, urlStates, userService) => {

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
                $scope.fnClickFacebookLogin = function () {
                    $scope.ngClickFacebookLogin();
                };

                /*
                * Event which is fired when cancel button is clicked.
                * */
                $scope.fnCancel = function () {
                    $scope.ngCancel();
                };

                /*
                * Event which is fired when basic registration button is clicked.
                * */
                $scope.fnClickBasicRegister = function () {
                    $scope.ngClickBasicRegister();
                };

                //#endregion
            }
        }
    });
};