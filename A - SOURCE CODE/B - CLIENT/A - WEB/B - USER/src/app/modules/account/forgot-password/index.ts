import {UrlStateConstant} from "../../../constants/url-state.constant";
import {StateProvider} from "@uirouter/angularjs";
import {module} from 'angular';

/* @ngInject */
export class ForgotPasswordModule {

    //#region Constructor

    public constructor($stateProvider: StateProvider) {

        $stateProvider
            .state(UrlStateConstant.accountForgotPasswordModuleName, {
                url: UrlStateConstant.accountForgotPasswordModuleUrl,
                templateProvider: ['$q', ($q) => {
                    // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                    return $q((resolve) => {
                        // lazy load the view
                        require.ensure([], () => {
                            require('../shared.scss');
                            resolve(require('./forgot-password.html'));
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
                                let ngModule = module('account.forgot-password', []);
                                const {ForgotPasswordController} =  require('./forgot-password.controller.ts');
                                ngModule.controller('forgotPasswordController', ForgotPasswordController);
                                $ocLazyLoad.load({name: ngModule.name});
                                resolve(ngModule.controller);
                            })
                        });
                    }
                },
                controller: 'forgotPasswordController',
                parent: UrlStateConstant.unauthorizedLayoutModuleName
            });
    }

    //#endregion

}