module.exports = (ngModule) => {
    ngModule.config(($stateProvider) => {

        // Import constants.
        const UrlStaticConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

        $stateProvider.state(UrlStaticConstant.dashboardModuleName, {
            url: UrlStaticConstant.dashboardModuleUrl,
            controller: 'mainDashboardController',
            parent: UrlStaticConstant.authorizedLayoutModuleName,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => {
                        require('./main.scss');
                        resolve(require('./main.html'))
                    });
                });
            }],
            resolve: {
                /*
                * Load login controller.
                * */
                loadDashboardController: ($q, $ocLazyLoad) => {
                    return $q((resolve) => {
                        require.ensure([], () => {
                            // load only controller module
                            let module = angular.module('main.dash-board', []);
                            require('./main.controller')(module);
                            $ocLazyLoad.load({name: module.name});
                            resolve(module.controller);
                        })
                    });
                }
            }
        });
    });
};