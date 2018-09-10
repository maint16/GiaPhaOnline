import {IController, IPromise, IQService} from "angular";
import {IFollowingTopicsScope} from "./following-topics.scope";
import {IFollowingTopicService} from "../../../../interfaces/services/following-topic-service.interface";
import {LoadFollowingTopicViewModel} from "../../../../view-models/following-topic/load-following-topic.view-model";
import {Pagination} from "../../../../models/pagination";
import {PaginationConstant} from "../../../../constants/pagination.constant";
import {IUiService} from "../../../../interfaces/services/ui-service.interface";
import {SearchResult} from "../../../../models/search-result";
import {FollowingTopic} from "../../../../models/entities/following-topic";
import {Topic} from "../../../../models/entities/topic";
import {LoadTopicViewModel} from "../../../../view-models/load-topic.view-model";
import {ITopicService} from "../../../../interfaces/services/topic-service.interface";
import {cloneDeep} from 'lodash';
import {StateService} from "@uirouter/core";
import {UrlStateConstant} from "../../../../constants/url-state.constant";

/* @ngInject */
export class PersonalFollowingTopicsController implements IController {

    //#region Properties

    private _mTopicIdToTopicMap: { [topicId: number]: Topic } = {};

    //#endregion

    //#region Constructor

    // Initialize controller with injectors.
    public constructor(public $scope: IFollowingTopicsScope,
                       public $state: StateService,
                       public $ui: IUiService,
                       public $followingTopic: IFollowingTopicService, public $topic: ITopicService) {

        // Property binding.
        let loadFollowingTopicCondition = new LoadFollowingTopicViewModel();
        let pagination = new Pagination();
        pagination.page = 1;
        pagination.records = PaginationConstant.followingTopics;
        loadFollowingTopicCondition.pagination = pagination;
        $scope.loadFollowingTopicCondition = loadFollowingTopicCondition;

        // Methods binding.
        $scope.ngOnInit = this._ngOnInit;
        $scope.ngOnPageChanged = this._ngOnPageChanged;
        $scope.ngOnTopicClicked = this._ngOnTopicClicked;
        $scope.ngGetTopic = this._ngGetTopic;
    }

    //#endregion

    //#region Methods

    // Called when component is initialized.
    private _ngOnInit = (): void => {

        // Reset pagination.
        let loadFollowingTopicCondition = this.$scope.loadFollowingTopicCondition;
        loadFollowingTopicCondition.pagination.page = 1;

        this._ngOnPageChanged();
    };

    // Called when page is changed.
    private _ngOnPageChanged = (): void => {
        // Add loading screen.
        this.$ui.blockAppUI();

        // Copy condition.
        let loadFollowingTopicCondition = cloneDeep(this.$scope.loadFollowingTopicCondition);
        this._loadFollowingTopics(loadFollowingTopicCondition)
            .finally(() => {
                this.$ui.unblockAppUI();
            });
    };

    // Convert topic id to topic.
    private _ngGetTopic = (topicId: number): Topic => {
        if (!this._mTopicIdToTopicMap[topicId])
            return null;

        return this._mTopicIdToTopicMap[topicId];
    };

    // Called when topic is clicked.
    private _ngOnTopicClicked = (topicId: number): void => {
        this.$state
            .go(UrlStateConstant.topicModuleName, {topicId: topicId});
    };

    // Load following topics.
    private _loadFollowingTopics = (condition: LoadFollowingTopicViewModel): IPromise<SearchResult<FollowingTopic>> => {

        // Mapping between topic id & topic.
        let mTopicIdToTopicMap: { [id: number]: Topic } = {};

        // Load following topic result.
        let loadFollowingTopicResult = new SearchResult<FollowingTopic>();

        return this.$followingTopic
            .loadFollowingCategories(condition)
            .then((loadFollowingTopic: SearchResult<FollowingTopic>) => {
                loadFollowingTopicResult = loadFollowingTopic;
                let followingTopics = loadFollowingTopicResult.records;

                // Get topics list.
                let loadTopicCondition = new LoadTopicViewModel();
                loadTopicCondition.ids = followingTopics.map(followingTopics => followingTopics.topicId);
                return this.$topic
                    .loadTopics(loadTopicCondition);
            })
            .then((loadTopicResult: SearchResult<Topic>) => {
                let topics = loadTopicResult.records;
                for (let topic of topics)
                    mTopicIdToTopicMap[topic.id] = topic;
            })
            .then(() => {
                this.$scope.loadedFollowingTopic = loadFollowingTopicResult;
                this._mTopicIdToTopicMap = mTopicIdToTopicMap;
                return loadFollowingTopicResult;
            })
    };

    //#endregion
}