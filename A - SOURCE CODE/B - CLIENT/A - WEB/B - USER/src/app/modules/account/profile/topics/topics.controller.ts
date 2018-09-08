import {IController, IPromise} from "angular";
import {IPersonalTopicsScope} from "./topics.scope";
import {LoadTopicViewModel} from "../../../../view-models/load-topic.view-model";
import {Pagination} from "../../../../models/pagination";
import {PaginationConstant} from "../../../../constants/pagination.constant";
import {User} from "../../../../models/entities/user";
import {IUiService} from "../../../../interfaces/services/ui-service.interface";
import {SearchResult} from "../../../../models/search-result";
import {ITopicService} from "../../../../interfaces/services/topic-service.interface";
import {Topic} from "../../../../models/entities/topic";

/* @ngInject */
export class PersonalTopicsController implements IController {

    //#region Properties


    //#endregion

    //#region Constructors

    // Initialize controller with injectors.
    public constructor(public $ui: IUiService,
                       public $topic: ITopicService,
                       public $scope: IPersonalTopicsScope){

        // Property binding.
        let loadPersonalTopicsCondition = new LoadTopicViewModel();
        let pagination = new Pagination();
        pagination.page = 1;
        pagination.records = PaginationConstant.topics;
        // loadPersonalTopicsCondition.ownerIds = [user.id];
        loadPersonalTopicsCondition.pagination = pagination;

        $scope.urlStateConstant = require('../../../../constants/url-state.constant').UrlStateConstant;
        $scope.loadPersonalTopicsCondition = loadPersonalTopicsCondition;

        // Methods binding.
        $scope.ngOnInit = this._ngOnInit;
        $scope.ngOnTopicsPageChanged = this._ngOnTopicsPageChanged;
    }

    //#endregion

    //#region Methods

    // Called when component is initialized.
    private _ngOnInit = (): void => {
        this._ngOnTopicsPageChanged();
    };

    // Called when topics page is changed.
    private _ngOnTopicsPageChanged = (): void => {

        // Block screen access.
        this.$ui.blockAppUI();

        this._loadPersonalTopics(this.$scope.loadPersonalTopicsCondition)
            .then((loadTopicsResult: SearchResult<Topic>) => {
                this.$scope.loadedPersonalTopics = loadTopicsResult;
            })
            .finally(() => {
                this.$ui.unblockAppUI();
            });
    };

    // Load personal topics using specific conditions.
    private _loadPersonalTopics = (condition: LoadTopicViewModel): IPromise<SearchResult<Topic>> => {
        if (!condition) {
            condition = new LoadTopicViewModel();
            let pagination = new Pagination();
            pagination.page = 1;
            pagination.records = PaginationConstant.topics;
            condition.pagination = pagination;
        }

        return this.$topic
            .loadTopics(condition);
    };

    //#endregion
}