import {IUserService} from '../interfaces/services/user-service.interface';
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import 'rxjs/add/observable/of';
import {ConfigUrlService} from '../constants/config-url-service.constant';
import {Observable} from 'rxjs/Rx';
import {SearchResult} from '../models/search-result';
import {ICategoryService} from '../interfaces/services/category-service.interface';
import {LoadCategoryViewModel} from '../view-models/category/load-category.view-model';
import {Category} from '../models/entities/category';
import {ConfigUrlCategoryServiceConstant} from '../constants/config-url-category-service.constant';
import {AddCategoryViewModel} from '../view-models/category/add-category.view-model';

@Injectable()
export class CategoryService implements ICategoryService {

  //#region Constructor

  /*
  * Initiate service with injectors.
  * */
  public constructor(public httpClient: HttpClient) {

  }

  //#endregion

  //#region Methods

  /*
  * Get profile information.
  * */

  // Load users by using specific conditions.
  public loadCategory(condition: LoadCategoryViewModel): Observable<SearchResult<Category>> {
      const url = `${ConfigUrlService.urlAPI}/${ConfigUrlCategoryServiceConstant.searchCategory}`;
    return this.httpClient.post<SearchResult<Category>>(url, condition);
  }
  public updateCategory(id: number, conditions: AddCategoryViewModel): Observable<Category>{
      let url = `${ConfigUrlService.urlAPI}/${ConfigUrlCategoryServiceConstant.editCategory}`;
    url = url.replace('{id}', String(id));
    return this.httpClient.put<Category>(url, conditions);
  }
  public addCategory(condition: AddCategoryViewModel){
    const url = `${ConfigUrlService.urlAPI}/${ConfigUrlCategoryServiceConstant.addCategory}`;
    let body = {
      name: condition.name,
      description: condition.description
    };
    return this.httpClient.post(url, body);
  }
  //#endregion

}
