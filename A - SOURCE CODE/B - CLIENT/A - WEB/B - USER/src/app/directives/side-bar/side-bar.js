module.exports = (ngModule) => {

    // Directive declaration.
    ngModule.directive('sideBar', () => {
        return {
            compile: () => {
                let pGetTemplatePromise = $q((resolve) => {
                    require.ensure([], () => resolve(require('./side-bar.html')));
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
            scope: null,
            controller: ($scope, urlStates) => {
                //#region Properties
                // Constants reflection.
                $scope.urlStates = urlStates;
                //#endregion
            }
        }
    });
};