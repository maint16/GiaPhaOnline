module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./navigation-bar.html');

    // Directive declaration.
    ngModule.directive('navigationBar', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            transclude: {
                userMenu: '?userMenu'
            },
            scope: {
                profile: '=?',
                ngApplicationTitle: '@',
                postNotifications: '=',
                totalPostNotifications: '=',

                ngClickLogin: '&',
                ngClickSignOut: '&'
            },
            controller: function($scope, urlStates){

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                //#endregion

                //#region Methods

                /*
                * Event which is fired when basic login is clicked.
                * */
                $scope.fnLogin = function(){
                    if ($scope.profile)
                        return;

                    $scope.ngClickLogin();
                };

                /*
                * Event which is fired when sign out button is clicked.
                * */
                $scope.fnSignOut = function(){
                    $scope.ngClickSignOut();
                };



                //#endregion
            }
        }
    });
};