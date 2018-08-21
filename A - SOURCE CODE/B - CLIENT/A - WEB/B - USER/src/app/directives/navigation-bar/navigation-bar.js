module.exports = (ngModule) => {
    // Directive declaration.
    ngModule.directive('navigationBar', () => {
        return {
            compile: () => {
                let pGetTemplatePromise = $q((resolve) => {
                    require.ensure([], () => resolve(require('./navigation-bar.html')));
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
            transclude: {
                userMenu: '?userMenu'
            },
            scope: {
                profile: '=?',
                ngUsers: '=?',
                ngApplicationTitle: '@',
                ngPostNotifications: '=?',
                ngPosts: '=?',
                totalPostNotifications: '=',

                ngClickLogin: '&',
                ngClickSignOut: '&'
            },
            controller: ($scope, urlStates, userRoleConstant) => {

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;
                $scope.userRoleConstant = userRoleConstant;

                //#endregion

                //#region Methods

                /*
                * Event which is fired when basic login is clicked.
                * */
                $scope.fnLogin = () => {
                    if ($scope.profile)
                        return;

                    $scope.ngClickLogin();
                };

                /*
                * Event which is fired when sign out button is clicked.
                * */
                $scope.fnSignOut = () => {
                    $scope.ngClickSignOut();
                };


                //#endregion
            }
        }
    });
};