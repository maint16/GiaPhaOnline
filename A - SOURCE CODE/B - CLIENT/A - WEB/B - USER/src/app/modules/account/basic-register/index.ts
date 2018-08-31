import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {module} from 'angular';

export class BasicRegisterModule {

    //#region Constructor

    public constructor($stateProvider: StateProvider) {
        $stateProvider.state(UrlStateConstant.accountRegisterModuleName, {
            url: UrlStateConstant.accountRegisterModuleUrl,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => {
                        require('../shared.scss');
                        resolve(require('./register.html'));
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
                            let ngModule = module('account.register', []);
                            const {BasicRegisterController} = require('./basic-register.controller.ts');
                            ngModule.controller('basicRegisterController', BasicRegisterController);
                            $ocLazyLoad.load({name: ngModule.name});
                            resolve(ngModule.controller);
                        })
                    });
                }
            },
            controller: 'basicRegisterController',
            parent: UrlStateConstant.unauthorizedLayoutModuleName
        })
    }

    //#endregion

}