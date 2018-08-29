import {IController} from "angular";
import {IUiService} from "../../../interfaces/services/ui-service.interface";
import {Topic} from "../../../models/entities/topic";
import {SearchResult} from "../../../models/search-result";
import {StateService} from '@uirouter/angularjs';
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {ITopicService} from "../../../interfaces/services/topic-service.interface";
import {LoadTopicViewModel} from "../../../view-models/load-topic.view-model";

import {cloneDeep} from 'lodash';
import {ITopicsScope} from "./topics.scope";

/* @ngInject */
export class TopicsController implements IController {

    //#region Constructor

    /*
    * Initialize controller with injectors.
    * */
    public constructor(public $scope: ITopicsScope,
                       public $state: StateService,
                       public $topic: ITopicService,
                       public $ui: IUiService) {

        this.$scope.loadTopicsResult = new SearchResult<Topic>();

        let loadTopicsCondition = new LoadTopicViewModel();
        loadTopicsCondition.pagination = null;
        this.$scope.loadTopicsCondition = loadTopicsCondition;

        this.$scope.ngOnInit = this._ngOnInit;
        this.$scope.ngOnAddTopic = this._ngOnAddTopic;
        this.$scope.ngOnTopicTitleClicked = this._ngOnTopicTitleClicked;
    }

    //#endregion

    //#region Methods

    /*
    * Called when component is initialized.
    * */
    private _ngOnInit = (): void => {
        this.$ui.blockAppUI();

        // Load topics using specific conditions.
        let loadTopicConditions: LoadTopicViewModel = cloneDeep(this.$scope.loadTopicsCondition);
        this.$topic
            .loadTopics(loadTopicConditions)
            .then((loadTopicsResult: SearchResult<Topic>) => {
                this.$scope.loadTopicsResult = loadTopicsResult;
            })
            .finally(() => {
                this.$ui.unblockAppUI();
            });
    };

    /*
    * Called when add topic button is clicked.
    * */
    private _ngOnAddTopic = (): void => {
        this.$state.go(UrlStateConstant.addTopicModuleName);
    };

    /*
    * Called when topic title is clicked.
    * */
    private _ngOnTopicTitleClicked = (id: number): void => {
        this.$state.go(UrlStateConstant.topicModuleName, {topicId: id});
    }

    //#endregion
}