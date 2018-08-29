import {LoadCategoryViewModel} from "../../view-models/load-category.view-model";
import {SearchResult} from "../../models/search-result";
import {Category} from "../../models/entities/category";
import {IPromise} from "angular";

export interface ICategoryService {

    //#region Methods

    /*
    * Load categories by using specific conditions.
    * */
    loadCategories(loadCategoriesCondition: LoadCategoryViewModel): IPromise<SearchResult<Category>>;

    //#endregion

}