import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {IQService, module} from 'angular';
import {ITopicService} from "../../../interfaces/services/topic-service.interface";
import {StateService} from '@uirouter/core';
import {TopicStateParam} from "../../../models/params/topic.state-param";
import {LoadTopicViewModel} from "../../../view-models/load-topic.view-model";
import {Pagination} from "../../../models/pagination";
import {SearchResult} from "../../../models/search-result";
import {Topic} from "../../../models/entities/topic";
import {TopicResolver} from "./topic.resolver";

/* @ngInject */
export class TopicModule {

    //#region Constructor

    /*
    * Initialize module with settings.
    * */
    public constructor(public $stateProvider: StateProvider) {
        $stateProvider
            .state(UrlStateConstant.topicModuleName, {
                url: UrlStateConstant.topicModuleUrl,
                controller: 'topicController',
                parent: UrlStateConstant.authorizedLayoutModuleName,
                templateProvider: ['$q', ($q) => {
                    // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                    return $q((resolve) => {
                        // lazy load the view
                        require.ensure([], () => {
                            require('./topic.scss');
                            resolve(require('./topic.html'));
                        });
                    });
                }],
                resolve: {
                    /*
                    * Load login controller.
                    * */
                    loadDashboardController: ($q: IQService, $ocLazyLoad: any) => {
                        return $q((resolve) => {
                            require.ensure([], () => {
                                // load only controller module
                                let ngModule = module('main.topic', []);
                                const {TopicController} = require('./topic.controller');
                                ngModule.controller('topicController', TopicController);
                                $ocLazyLoad.load({name: ngModule.name});
                                resolve(ngModule.controller);
                            })
                        });
                    },

                    loadTopicResolver: ($topic: ITopicService,
                            $state: StateService, $stateParams: Map<string, object>) => {

                        // Get topic id.
                        let topicId: number = parseInt($stateParams['topicId']);
                        if (!topicId)
                            throw 'No topic has been specified';

                        //#region Load topic

                        let loadTopicsCondition = new LoadTopicViewModel();
                        let pagination = new Pagination();
                        pagination.records = 1;
                        pagination.page = 1;

                        loadTopicsCondition.ids = [topicId];
                        loadTopicsCondition.pagination = pagination;

                        let topicResolver = new TopicResolver();

                        return $topic
                            .loadTopics(loadTopicsCondition)
                            .then((loadTopicsResult: SearchResult<Topic>) => {

                                let topics = loadTopicsResult.records;
                                if (!topics || !topics.length)
                                    throw 'No topic has been found';

                                topicResolver.topic = topics[0];
                                return topicResolver;
                            });

                        //#endregion
                    }
                }
            });
    }

    //#endregion

}