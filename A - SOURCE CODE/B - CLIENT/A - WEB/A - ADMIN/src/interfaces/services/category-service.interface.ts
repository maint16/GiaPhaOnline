import {Observable} from 'rxjs/Observable';
import {SearchResult} from '../../models/search-result';
import {Category} from '../../models/entities/category';
import {LoadCategoryViewModel} from '../../view-models/category/load-category.view-model';
import {AddCategoryViewModel} from '../../view-models/category/add-category.view-model';
export interface ICategoryService {
  //#region Methods
  /*
 * Get profile information.
 * */
  // Load users from api end-point.
  loadCategory(conditions: LoadCategoryViewModel): Observable<SearchResult<Category>>;
  addCategory(conditions: AddCategoryViewModel);
  updateCategory(id: number, conditions: AddCategoryViewModel): Observable<Category>;
}
