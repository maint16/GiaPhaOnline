import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {module} from 'angular';

export class UnauthorizedLayoutModule {

    //#region Constructor

    /*
    * Initialize module with injectors.
    * */
    public constructor($stateProvider: StateProvider){

        $stateProvider
            .state(UrlStateConstant.unauthorizedLayoutModuleName, {
            controller: 'unauthorizedLayoutController',
            abstract: true,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => resolve(require('./unauthorized-layout.html')));
                });
            }],
            resolve: {
                /*
                * Load login controller.
                * */
                loadController: ($q, $ocLazyLoad) => {
                    return $q((resolve) => {
                        require.ensure([], () => {
                            // load only controller module
                            let ngModule = module('shared.unauthorized-layout', []);
                            const {UnauthorizedLayoutController} = require('./unauthorized-layout.controller.ts');
                            ngModule.controller('unauthorizedLayoutController', UnauthorizedLayoutController);
                            $ocLazyLoad.load({name: ngModule.name});
                            resolve(ngModule.controller);
                        })
                    });
                }
            },
        })
    }

    //#endregion

}