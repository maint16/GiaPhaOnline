import {Observable} from 'rxjs/Observable';
import {SearchResult} from '../../models/search-result';
import {LoadCategoryGroupViewModel} from '../../view-models/category-group/load-category-group.view-model';
import {CategoryGroup} from '../../models/entities/category-group';
import {AddCategoryGroup} from '../../view-models/category-group/add-category-group.view-model';
export interface ICategoryGroupService {
  //#region Methods
  /*
 * Get profile information.
 * */
  // Load users from api end-point.
  loadCategoryGroup(conditions: LoadCategoryGroupViewModel): Observable<SearchResult<CategoryGroup>>;
  addCategoryGroup(conditions: AddCategoryGroup);
  updateCategoryGroup(id: number, conditions: AddCategoryGroup): Observable<CategoryGroup>;
}
