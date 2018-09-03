import {LoadCategoryGroupViewModel} from "../../view-models/load-category-group.view-model";
import {SearchResult} from "../../models/search-result";
import {CategoryGroup} from "../../models/entities/category-group";
import {IPromise} from "angular";

export interface ICategoryGroupService {

    //#region Methods

    /*
    * Load topics groups by using specific conditions.
    * */
    loadCategoryGroups(loadCategoryGroupsCondition: LoadCategoryGroupViewModel): IPromise<SearchResult<CategoryGroup>>

    //#endregion

}