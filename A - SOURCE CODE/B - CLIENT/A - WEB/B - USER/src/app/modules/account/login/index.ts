import {module} from 'angular'
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {StateProvider} from "@uirouter/angularjs";

export class LoginModule {

    //#region Constructor

    public constructor($stateProvider: StateProvider) {
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
                            const {LoginController} = require('./login.controller.ts');
                            let ngModule = module('account.login', []);
                            ngModule.controller('loginController', LoginController);
                            $ocLazyLoad.load({name: ngModule.name});
                            resolve(ngModule.controller);
                        })
                    });
                }
            },
            controller: 'loginController',
            parent: UrlStateConstant.unauthorizedLayoutModuleName
        })
    }

    //#endregion
}