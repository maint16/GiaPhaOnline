module.exports = (ngModule) => {

    // Directive declaration.
    ngModule.directive('userPicker', function () {
        return {
            compile: () => {
                let pGetTemplatePromise = $q((resolve) => {
                    require.ensure([], () => resolve(require('./user-picker.html')));
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
                ngIsHeaderAvailable: '=',
                ngSelectUser: '&',
                ngClickCancel: '&'
            },
            controller: ($scope, $translate, $compile,
                         userService, commonService,
                         appSettingConstant,
                         DTOptionsBuilder, DTColumnBuilder) => {

                // Whether component is busy or not.
                $scope.bIsBusy = false;

                // Buffer data.
                $scope.buffer = {
                    users: {}
                };

                //#region Properties

                /*
                * Model which is for information binding.
                * */
                $scope.model = {
                    email: null,
                    nickname: null
                };

                /*
                * Get user result.
                * */
                $scope.getUserResult = {
                    records: null,
                    total: 0
                };

                //#endregion

                //#region Methods

                /*
                * Fired when directive start to be initialized.
                * */
                $scope.init = function () {

                };

                /*
                * Fired when user is selected.
                * */
                $scope.selectUser = function (id) {
                    $scope.ngSelectUser({user: $scope.buffer.users[id]});
                };

                /*
                * Event which is fired when cancel button is clicked.
                * */
                $scope.cancel = function () {
                    $scope.ngClickCancel();
                };

                //#endregion
            }
        }
    });
};