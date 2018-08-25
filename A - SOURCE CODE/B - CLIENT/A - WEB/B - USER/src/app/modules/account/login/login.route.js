module.exports = (ngModule) => {

    //#region Module configs.

    /*
    * Module configuration.
    * */
    ngModule.config(($stateProvider) => {
        // Get state parameter.
        const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

        $stateProvider.state(UrlStateConstant.loginModuleName, {
            url: UrlStateConstant.loginModuleUrl,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => {
                        require('../shared.scss');
                        resolve(require('./login.html'));
                    });
                });
            }],
            resolve: {
                /*
                * Load login controller.
                * */
                loadLoginController: ($q, $ocLazyLoad) => {
                    return $q((resolve) => {
                        require.ensure([], () => {
                            // load only controller module
                            let module = angular.module('account.login', []);
                            require('./login.controller')(module);
                            $ocLazyLoad.load({name: module.name});
                            resolve(module.controller);
                        })
                    });
                }
            },
            controller: 'loginController',
            parent: UrlStateConstant.unauthorizedLayoutModuleName
        })
    });

    //#endregion
};