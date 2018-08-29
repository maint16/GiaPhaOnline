import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {module} from 'angular';

export class TopicsModule {

    //region Constructors

    public constructor($stateProvider: StateProvider){

        $stateProvider.state(UrlStateConstant.categoryTopicModuleName, {
            url: UrlStateConstant.categoryTopicModuleUrl,
            controller: 'topicsController',
            parent: UrlStateConstant.authorizedLayoutModuleName,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => {
                        require('./topics.scss');
                        resolve(require('./topics.html'));
                    });
                });
            }],
            resolve: {
                /*
                * Load login controller.
                * */
                loadDashboardController: ['$q', '$ocLazyLoad', ($q, $ocLazyLoad) => {
                    return $q((resolve) => {
                        require.ensure([], () => {
                            // load only controller module
                            let ngModule = module('main.topics', []);
                            const {TopicsController} = require('.//topics.controller');
                            ngModule.controller('topicsController', TopicsController);
                            $ocLazyLoad.load({name: ngModule.name});
                            resolve(ngModule.controller);
                        })
                    });
                }]
            }
        });
    }

    //#endregion

}