module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./navigation-bar.html');

    // Directive declaration.
    ngModule.directive('navigationBar', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                profile: '=?',

                ngClickBasicLogin: '&',
                ngClickGoogleLogin: '&',
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
                $scope.fnBasicLogin = function(){
                    $scope.ngClickBasicLogin();
                };

                $scope.fnGoogleSignIn = function(){
                    $scope.ngClickGoogleLogin();
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