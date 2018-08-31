import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {module} from 'angular';
import {ProfileController} from "./profile.controller";

export class ProfileModule {

    //#region Constructor

    /*
    * Initialize module with injectors.
    * */
    public constructor($stateProvider: StateProvider){
        $stateProvider.state(UrlStateConstant.profileModuleName, {
            url: UrlStateConstant.profileModuleUrl,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => resolve(require('./profile.html')));
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
                            let ngModule = module('account.profile', []);

                            const {ProfileController} = require('./profile.controller.ts');
                            ngModule.controller('profileController', ProfileController);
                            $ocLazyLoad.load({name: ngModule.name});
                            resolve(ngModule.controller);
                        })
                    });
                }
            },
            controller: 'profileController',
            parent: UrlStateConstant.authorizedLayoutModuleName
        });
    }

    //#endregion

}