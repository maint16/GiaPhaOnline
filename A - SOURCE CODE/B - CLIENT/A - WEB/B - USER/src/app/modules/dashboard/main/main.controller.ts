import {IController} from "angular";
import {IUiService} from "../../../interfaces/services/ui-service.interface";
import {ICategoryGroupService} from "../../../interfaces/services/category-group-service.interface";
import {ICategoryService} from "../../../interfaces/services/category-service.interface";
import {LoadCategoryViewModel} from "../../../view-models/load-category.view-model";

import {cloneDeep} from 'lodash';
import {IMainScope} from "./main.scope";
import {LoadCategoryGroupViewModel} from "../../../view-models/load-category-group.view-model";
import {Pagination} from "../../../models/pagination";
import {PaginationConstant} from "../../../constants/pagination.constant";
import {SearchResult} from "../../../models/search-result";
import {CategoryGroup} from "../../../models/entities/category-group";
import {Category} from "../../../models/entities/category";
import {StateService} from "@uirouter/core";
import {UrlStateConstant} from "../../../constants/url-state.constant";

/* @ngInject */
export class MainController implements IController {

    //#region Properties


    //#endregion

    //#region Constructor

    public constructor(public $state: StateService,
                       public $ui: IUiService,
                       public $categoryGroup: ICategoryGroupService,
                       public $category: ICategoryService,
                       public $scope: IMainScope) {

        // Property binding.
        let loadCategoryGroupsCondition = new LoadCategoryGroupViewModel();
        let pagination = new Pagination();
        pagination.page = 1;
        pagination.records = PaginationConstant.categoryReplies;

        loadCategoryGroupsCondition.pagination = pagination;
        $scope.loadCategoryGroupsCondition = loadCategoryGroupsCondition;
        $scope.mCategoryGroupIdToCategoriesMap = {};

        // Methods mapping.
        $scope.ngOnInit = this._ngOnInit;
        $scope.ngOnCategoryClicked = this._ngOnCategoryClicked;
    }

    //#endregion

    //#region Methods

    /*
    * Called when component is initialized.
    * */
    private _ngOnInit = (): void => {

        // Copy loading condition.
        let loadCategoryGroupCondition = cloneDeep(this.$scope.loadCategoryGroupsCondition);

        // Block app ui.
        this.$ui.blockAppUI();

        // Initialize search condition.
        let loadCategoryGroupsResult = new SearchResult<CategoryGroup>();
        let mCategoryGroupIdToCategoriesMap: { [id: number]: Array<Category> } = {};

        this.$categoryGroup
            .loadCategoryGroups(loadCategoryGroupCondition)
            .then((result: SearchResult<CategoryGroup>) => {
                loadCategoryGroupsResult = result;
                return result.records
            })
            .then((categoryGroups: Array<CategoryGroup>) => {
                // Get the list of categories ids.
                let loadCategoriesCondition = new LoadCategoryViewModel();
                loadCategoriesCondition.pagination = null;
                loadCategoriesCondition.categoryGroupIds = categoryGroups.map((categoryGroup) => categoryGroup.id);

                return this.$category
                    .loadCategories(loadCategoriesCondition)
                    .then((loadCategoriesResult: SearchResult<Category>) => {
                        return loadCategoriesResult.records;
                    });
            })
            .then((categories: Array<Category>) => {

                for (let category of categories) {
                    if (!mCategoryGroupIdToCategoriesMap[category.categoryGroupId])
                        mCategoryGroupIdToCategoriesMap[category.categoryGroupId] = [];

                    mCategoryGroupIdToCategoriesMap[category.categoryGroupId].push(category);
                }

                this.$scope.loadCategoryGroupsResult = loadCategoryGroupsResult;
                this.$scope.mCategoryGroupIdToCategoriesMap = mCategoryGroupIdToCategoriesMap;
            })
            .finally(() => {
                this.$ui.unblockAppUI();
            });
    };

    /*
    * Called when category is clicked.
    * */
    private _ngOnCategoryClicked = (categoryId: number): void => {
        this.$state.go(UrlStateConstant.categoryTopicModuleName, {categoryId: categoryId});
    };

    //#endregion

    //#endregion
}