module.exports = (ngModule) => {

    // Import constant.
    const UrlStateConstants = require('../../constants/url-state.constant').UrlStateConstant;

    // Directive declaration.
    ngModule.directive('navigationBar', ($q, $compile) => {
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
            controller: ($scope, userRoleConstant) => {

                //#region Properties

                // Constants reflection.
                $scope.userRoleConstant = userRoleConstant;
                $scope.urlStateConstants = UrlStateConstants;


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