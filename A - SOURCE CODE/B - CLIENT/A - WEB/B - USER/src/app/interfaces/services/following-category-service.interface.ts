import {LoadFollowingCategoryViewModel} from "../../view-models/following-category/load-following-category.view-model";
import {SearchResult} from "../../models/search-result";
import {FollowingCategory} from "../../models/entities/following-category";
import {IPromise} from "angular";

export interface IFollowingCategoryService {

    //#region Methods

    // Load following categories.
    loadFollowingCategories(condition: LoadFollowingCategoryViewModel): IPromise<SearchResult<FollowingCategory>>

    //#endregion
}