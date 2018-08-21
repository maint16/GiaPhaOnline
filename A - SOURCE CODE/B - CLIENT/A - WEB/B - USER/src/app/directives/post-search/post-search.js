module.exports = (ngModule) => {
    // Directive declaration.
    ngModule.directive('postSearch', function () {
        return {
            compile: () => {
                let pGetTemplatePromise = $q((resolve) => {
                    require.ensure([], () => resolve(require('./post-search.html')));
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
                ngClickCancel: '&',
            },
            controller: ($scope, urlStates) => {

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;

                // Model which is for information binding.
                $scope.model = {
                    title: null
                };
                //#endregion

                //#region Methods

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