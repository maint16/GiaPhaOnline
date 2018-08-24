module.exports = (ngModule) => {
    // Directive declaration.
    ngModule.directive('appFooter', ($q, $compile) => {
        return {
            compile: () => {
                let pGetTemplatePromise = $q((resolve) => {
                    require.ensure([], () => resolve(require('./footer.html')));
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
            },
            controller: ($scope) => {

            }
        }
    });
};