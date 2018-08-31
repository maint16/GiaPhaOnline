import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {module} from 'angular';
import {MainController} from "./main.controller";

export class MainModule {

    //#region Constructor

    /*
    * Initialize module with injectors.
    * */
    public constructor($stateProvider: StateProvider) {

        $stateProvider.state(UrlStateConstant.dashboardModuleName, {
            url: UrlStateConstant.dashboardModuleUrl,
            controller: 'mainController',
            parent: UrlStateConstant.authorizedLayoutModuleName,
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
                loadController: ($q, $ocLazyLoad) => {
                    return $q((resolve) => {
                        require.ensure([], () => {
                            // load only controller module
                            let ngModule = module('main.dashboard', []);
                            const {MainController} = require('./main.controller.ts');
                            ngModule.controller('mainController', MainController);
                            $ocLazyLoad.load({name: ngModule.name});
                            resolve(ngModule.controller);
                        })
                    });
                }
            }
        });
    }

    //#endregion

}