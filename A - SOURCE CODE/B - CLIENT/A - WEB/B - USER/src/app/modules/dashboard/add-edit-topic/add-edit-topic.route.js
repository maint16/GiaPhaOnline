module.exports = (ngModule) => {
    ngModule.config(($stateProvider) => {

        // Import constants.
        const UrlStaticConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

        //#region Add topic

        $stateProvider
            .state(UrlStaticConstant.addTopicModuleName, {
                url: UrlStaticConstant.addTopicModuleUrl,
                controller: 'addEditTopicController',
                parent: UrlStaticConstant.authorizedLayoutModuleName,
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
                    loadDashboardController: ($q, $ocLazyLoad) => {
                        return $q((resolve) => {
                            require.ensure([], () => {
                                // load only controller module
                                let module = angular.module('main.add-edit-topic', []);
                                require('./add-edit-topic.controller')(module);
                                $ocLazyLoad.load({name: module.name});
                                resolve(module.controller);
                            })
                        });
                    }
                }
            });

        //#endregion

        //#region Edit topic

        $stateProvider
            .state(UrlStaticConstant.editTopicModuleName, {
                url: UrlStaticConstant.editTopicModuleUrl,
                controller: 'addEditTopicController',
                parent: UrlStaticConstant.authorizedLayoutModuleName,
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
                    loadDashboardController: ($q, $ocLazyLoad) => {
                        return $q((resolve) => {
                            require.ensure([], () => {
                                // load only controller module
                                let module = angular.module('main.add-edit-topic', []);
                                require('./add-edit-topic.controller')(module);
                                $ocLazyLoad.load({name: module.name});
                                resolve(module.controller);
                            })
                        });
                    }
                }
            });

        //#endregion
    });
};