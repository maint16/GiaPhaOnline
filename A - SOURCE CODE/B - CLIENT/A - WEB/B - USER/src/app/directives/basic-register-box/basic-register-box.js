module.exports = (ngModule) => {

    // Directive declaration.
    ngModule.directive('basicRegisterBox', () => {
        return {
            compile: () => {
                let pGetTemplatePromise = $q((resolve) => {
                    require.ensure([], () => resolve(require('./basic-register-box.html')));
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
                ngClickRegister: '&',
                ngClickCancel: '&'
            },
            controller: ($scope, userService) => {

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