import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {module} from 'angular';

export class AddEditTopicModule {

    //#region Constructor

    /*
    * Initialize module with injectors.
    * */
    public constructor($stateProvider: StateProvider) {

        //#region Add topic

        $stateProvider
            .state(UrlStateConstant.addTopicModuleName, {
                url: UrlStateConstant.addTopicModuleUrl,
                controller: 'addEditTopicController',
                parent: UrlStateConstant.authorizedLayoutModuleName,
                templateProvider: ['$q', ($q) => {
                    // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                    return $q((resolve) => {
                        // lazy load the view
                        require.ensure([], () => resolve(require('./add-edit-topic.html')));
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
                                let ngModule = module('main.add-edit-topic', []);
                                require('./add-edit-topic.controller')(ngModule);
                                $ocLazyLoad.load({name: ngModule.name});
                                resolve(ngModule.controller);
                            })
                        });
                    }
                }
            });

        //#endregion

        //#region Edit topic

        $stateProvider
            .state(UrlStateConstant.editTopicModuleName, {
                url: UrlStateConstant.editTopicModuleUrl,
                controller: 'addEditTopicController',
                parent: UrlStateConstant.authorizedLayoutModuleName,
                templateProvider: ['$q', ($q) => {
                    // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                    return $q((resolve) => {
                        // lazy load the view
                        require.ensure([], () => resolve(require('./add-edit-topic.html')));
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
                                let ngModule = module('main.add-edit-topic', []);
                                require('./add-edit-topic.controller')(ngModule);
                                $ocLazyLoad.load({name: ngModule.name});
                                resolve(ngModule.controller);
                            })
                        });
                    }
                }
            });

        //#endregion

    }

    //#endregion

}