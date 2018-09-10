import {IController} from "angular";
import {IFollowingCategoriesScope} from "./following-categories.scope";
import {IFollowingCategoryService} from "../../../../interfaces/services/following-category-service.interface";
import {IUiService} from "../../../../interfaces/services/ui-service.interface";
import {cloneDeep} from 'lodash';
import {SearchResult} from "../../../../models/search-result";
import {FollowingCategory} from "../../../../models/entities/following-category";
import {ICategoryService} from "../../../../interfaces/services/category-service.interface";
import {LoadCategoryViewModel} from "../../../../view-models/load-category.view-model";
import {LoadFollowingCategoryViewModel} from "../../../../view-models/following-category/load-following-category.view-model";
import {User} from "../../../../models/entities/user";
import {Category} from "../../../../models/entities/category";
import {Pagination} from "../../../../models/pagination";
import {PaginationConstant} from "../../../../constants/pagination.constant";

/* @ngInject */
export class FollowingCategoriesController implements IController {

    //#region Properties

    // Mapping of category id - category.
    private _mCategoryIdToCategoryMap: {[categoryId: number]: Category} = {};

    //#endregion

    //#region Constructor

    // Initialize controller with injector.
    public constructor(public $scope: IFollowingCategoriesScope,
                       public $ui: IUiService,
                       public $followingCategory: IFollowingCategoryService,
                       public $category: ICategoryService,
                       public profile: User) {
        // Property binding.
        let loadFollowingCategoryCondition = new LoadFollowingCategoryViewModel();
        let pagination = new Pagination();
        pagination.page = 1;
        pagination.records = PaginationConstant.followingCategories;
        loadFollowingCategoryCondition.followerIds = [profile.id];
        loadFollowingCategoryCondition.pagination = pagination;
        $scope.loadFollowingCategoriesCondition = loadFollowingCategoryCondition;

        // Methods binding.
        $scope.ngOnInit = this._ngOnInit;
        $scope.ngOnPageChanged = this._ngOnPageChanged;
        $scope.ngGetCategory = this._ngGetCategory;
    }

    //#endregion

    //#region Methods

    // Called when component is initialized.
    private _ngOnInit = (): void => {
        this._ngOnPageChanged();
    };

    // Called when page is changed.
    private _ngOnPageChanged = (): void => {
        // Block ui.
        this.$ui.blockAppUI();

        // Copy search condition.
        let condition = cloneDeep(this.$scope.loadFollowingCategoriesCondition);

        // Load following category result.
        let loadFollowingCategoryResult = new SearchResult<FollowingCategory>();

        // Initialize mapping.
        let mCategoryIdToCategoryMap: {[categoryId: number]: Category} = {};

        this.$followingCategory
            .loadFollowingCategories(condition)
            .then((loadFollowingCategoriesResult: SearchResult<FollowingCategory>) => {
                loadFollowingCategoryResult = loadFollowingCategoriesResult;

                let followingCategories = loadFollowingCategoriesResult.records;
                let loadCategoriesCondition = new LoadCategoryViewModel();
                loadCategoriesCondition.ids = followingCategories.map(followingCategory => followingCategory.categoryId);

                return this.$category
                    .loadCategories(loadCategoriesCondition);
            })
            .then((loadCategoriesResult: SearchResult<Category>) => {
                let categories = loadCategoriesResult.records;
                for (let category of categories)
                    mCategoryIdToCategoryMap[category.id] = category;
            })
            .finally(() => {
                this.$ui.unblockAppUI();
                this.$scope.loadFollowingCategoriesResult = loadFollowingCategoryResult;
                this._mCategoryIdToCategoryMap = mCategoryIdToCategoryMap;
            });
    };

    // Get category from id.
    private _ngGetCategory = (id: number): Category => {
        let category = this._mCategoryIdToCategoryMap[id];
        if (!category)
            return null;

        return category;
    };

    //#endregion

}