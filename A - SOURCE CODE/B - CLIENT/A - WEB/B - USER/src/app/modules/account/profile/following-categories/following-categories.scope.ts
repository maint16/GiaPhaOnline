import {IScope} from "angular";
import {SearchResult} from "../../../../models/search-result";
import {FollowingCategory} from "../../../../models/entities/following-category";
import {LoadFollowingCategoryViewModel} from "../../../../view-models/following-category/load-following-category.view-model";
import {Category} from "../../../../models/entities/category";

export interface IFollowingCategoriesScope extends IScope{

    //#region Properties

    // Load following categories result.
    loadFollowingCategoriesResult: SearchResult<FollowingCategory>;

    // Load following categories condition.
    loadFollowingCategoriesCondition: LoadFollowingCategoryViewModel;

    //#endregion

    //#region Methods

    // Called when component is initialized.
    ngOnInit(): void;

    // Called when load following categories is changed.
    ngOnPageChanged(): void;

    // Get category from id.
    ngGetCategory(id: number): Category;

    //#endregion

}