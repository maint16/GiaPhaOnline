import {SearchCategoriesViewModel} from "../../../viewmodels/category/search-categories.view-model";
import {Category} from "../../../models/entities/Category";
import {Response} from '@angular/http';

export interface ICategoryService {

  //#region Methods

  /*
  * Find categories by using specific conditions.
  * */
  getCategories(conditions: SearchCategoriesViewModel): Promise<Response>;

  /*
  * Find categories by using specific conditions and delete 'em all.
  * */
  deleteCategories(conditions: SearchCategoriesViewModel): Promise<Response>;

  /*
  * Find categories by using index and update their information.
  * */
  editCategoryDetails(id: number, category: Category): Promise<Response>;

  /*
  * Initiate category into system.
  * */
  initiateCategory(category: any): Promise<Response>;

  //#endregion
}
