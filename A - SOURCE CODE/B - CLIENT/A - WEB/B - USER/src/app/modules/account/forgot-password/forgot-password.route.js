module.exports = (ngModule) => {

    //#region Module configs.

    /*
    * Module configuration.
    * */
    ngModule.config(($stateProvider) => {
        // Get state parameter.
        const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

        $stateProvider.state(UrlStateConstant.accountForgotPasswordModuleName, {
            url: UrlStateConstant.accountForgotPasswordModuleUrl,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => resolve(require('./forgot-password.html')));
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
                            let module = angular.module('account.forgot-password', []);
                            require('./forgot-password.controller')(module);
                            $ocLazyLoad.load({name: module.name});
                            resolve(module.controller);
                        })
                    });
                }
            },
            controller: 'forgotPasswordController',
            parent: UrlStateConstant.unauthorizedLayoutModuleName
        })
    });

    //#endregion
};