import {IUserService} from '../interfaces/services/user-service.interface';
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import 'rxjs/add/observable/of';
import {ConfigUrlService} from '../constants/config-url-service.constant';
import {Observable} from 'rxjs/Rx';
import {SearchResult} from '../models/search-result';
import {ICategoryGroupService} from '../interfaces/services/category-group-service.interface';
import {CategoryGroup} from '../models/entities/category-group';
import {ConfigUrlCategoryGroupServiceConstant} from '../constants/config-url-category-service.constant';
import {LoadCategoryGroupViewModel} from '../view-models/category-group/load-category-group.view-model';
import {AddCategoryGroup} from '../view-models/category-group/add-category-group.view-model';

@Injectable()
export class CategoryGroupService implements ICategoryGroupService {

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
  public loadCategoryGroup(condition: LoadCategoryGroupViewModel): Observable<SearchResult<CategoryGroup>> {
      const url = `${ConfigUrlService.urlAPI}/${ConfigUrlCategoryGroupServiceConstant.searchCategoryGroup}`;
    return this.httpClient.post<SearchResult<CategoryGroup>>(url, condition);
  }
  public updateCategoryGroup(id: number, conditions: AddCategoryGroup): Observable<CategoryGroup>{
      const url = `${ConfigUrlService.urlAPI}/${ConfigUrlCategoryGroupServiceConstant.editCategoryGroup}`;
    url = url.replace('{id}', String(id));
    return this.httpClient.put<CategoryGroup>(url, conditions);
  }
  public addCategoryGroup(condition: AddCategoryGroup){
    const url = `${ConfigUrlService.urlAPI}/${ConfigUrlCategoryGroupServiceConstant.addCategoryGroup}`;
    let body = {
      name: condition.name,
      description: condition.description
    };
    return this.httpClient.post(url, body);
  }
  //#endregion

}
