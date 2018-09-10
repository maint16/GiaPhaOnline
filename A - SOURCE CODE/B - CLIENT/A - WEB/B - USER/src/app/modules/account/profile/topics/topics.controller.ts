import {IController, IPromise, IQService} from "angular";
import {IPersonalTopicsScope} from "./topics.scope";
import {LoadTopicViewModel} from "../../../../view-models/load-topic.view-model";
import {Pagination} from "../../../../models/pagination";
import {PaginationConstant} from "../../../../constants/pagination.constant";
import {IUiService} from "../../../../interfaces/services/ui-service.interface";
import {SearchResult} from "../../../../models/search-result";
import {ITopicService} from "../../../../interfaces/services/topic-service.interface";
import {Topic} from "../../../../models/entities/topic";
import {Category} from "../../../../models/entities/category";
import {ICategoryService} from "../../../../interfaces/services/category-service.interface";
import {LoadCategoryViewModel} from "../../../../view-models/load-category.view-model";
import {IUserService} from "../../../../interfaces/services/user-service.interface";
import {StateService} from "@uirouter/core";
import {UrlStateConstant} from "../../../../constants/url-state.constant";

/* @ngInject */
export class PersonalTopicsController implements IController {

    //#region Properties

    //#endregion

    //#region Constructors

    // Initialize controller with injectors.
    public constructor(public $ui: IUiService,
                       public $topic: ITopicService, public $category: ICategoryService, public $user: IUserService,
                       public $q: IQService, public $state: StateService,
                       public $scope: IPersonalTopicsScope) {

        // Property binding.
        let loadPersonalTopicsCondition = new LoadTopicViewModel();
        let pagination = new Pagination();
        pagination.page = 1;
        pagination.records = PaginationConstant.topics;
        // loadPersonalTopicsCondition.ownerIds = [user.id];
        loadPersonalTopicsCondition.pagination = pagination;

        $scope.urlStateConstant = UrlStateConstant;
        $scope.loadPersonalTopicsCondition = loadPersonalTopicsCondition;

        // Methods binding.
        $scope.ngOnInit = this._ngOnInit;
        $scope.ngOnTopicsPageChanged = this._ngOnTopicsPageChanged;
        $scope.ngOnViewTopicClicked = this._ngOnViewTopicClicked;
        $scope.ngOnEditTopicClicked = this._ngOnEditTopicClicked;
        $scope.ngOnDeleteTopicClicked = this._ngOnDeleteTopicClicked;
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

    // Called when view topic is clicked.
    private _ngOnViewTopicClicked = (topic: Topic): void => {
        this.$state
            .go(UrlStateConstant.topicModuleName, {topicId: topic.id});
    };

    // Called when edit topic is clicked.
    private _ngOnEditTopicClicked = (topic: Topic): void => {
        this.$state
            .go(UrlStateConstant.editTopicModuleName, {topicId: topic.id});
    };

    // Called when delete topic is clicked.
    private _ngOnDeleteTopicClicked = (topic: Topic): void => {
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

        // Id to category map.
        let mIdToCategoryMap: { [id: number]: Category } = {};

        return this.$topic
            .loadTopics(condition)
            .then((loadTopicsResult: SearchResult<Topic>) => {

                // Get list of categories.
                let topics: Topic[] = loadTopicsResult.records;

                // Initialize promises list.
                let promises: IPromise<any>[] = [];

                //#region Load categories.

                // Load categories condition.
                let loadCategoriesCondition = new LoadCategoryViewModel();
                loadCategoriesCondition.ids = topics.map((topic: Topic) => topic.categoryId);
                promises[0] = this.$category
                    .loadCategories(loadCategoriesCondition)
                    .then((loadCategoriesResult: SearchResult<Category>) => {
                        let categories = loadCategoriesResult.records;
                        for (let category of categories)
                            mIdToCategoryMap[category.id] = category;
                    });

                //#endregion

                return this.$q
                    .all(promises)
                    .then(() => loadTopicsResult);
            })
            .then((loadTopicsResult: SearchResult<Topic>) => {
                this.$scope.mIdToCategoryMap = mIdToCategoryMap;
                return loadTopicsResult;
            });

    };

    //#endregion
}