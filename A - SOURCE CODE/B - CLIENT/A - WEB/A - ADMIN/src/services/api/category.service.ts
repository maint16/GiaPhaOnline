import {Injectable, Inject} from '@angular/core';
import {SearchCategoriesViewModel} from "../../viewmodels/category/search-categories.view-model";
import {Category} from "../../models/entities/category";
import {IApiService} from "../../interfaces/services/api/api-service.interface";
import {environment} from "../../environments/environment";
import {ApiUrl} from "../../constants/api-url";
import {ICategoryService} from "../../interfaces/services/api/category-service.interface";
import {Response} from '@angular/http';

/*
 * Service which handles category business.
 * */
@Injectable()
export class CategoryService implements ICategoryService {

  //#region Constructor

  /*
  * Initiate instance of category service.
  * */
  public constructor(@Inject("IApiService") public apiService: IApiService) {
  }

  //#endregion

  //Region Methods

  /*
  * Find categories by using specific conditions.
  * */
  public getCategories(conditions: SearchCategoriesViewModel): Promise<Response> {

    // Page page should be decrease by one.
    return this.apiService.post(environment.baseUrl, ApiUrl.getCategories,
      null,
      conditions)
      .toPromise();
  }

  /*
  * Find categories by using specific conditions and delete 'em.
  * */
  public deleteCategories(conditions: SearchCategoriesViewModel): Promise<Response> {
    // Request to api to obtain list of available categories in system.
    return this.apiService.delete(
      environment.baseUrl,
      ApiUrl.deleteCategory,
      null,
      conditions)
      .toPromise();
  }

  /*
  * Change category detail information by searching its page.
  * */
  public editCategoryDetails(id: number, category: Category): Promise<Response> {
    // Request to api to obtain list of available categories in system.
    return this.apiService.put(environment.baseUrl, ApiUrl.editCategoryDetails,
      null, category)
      .toPromise();
  }

  /*
  * Initiate category into system.
  * */
  public initiateCategory(category: any): Promise<Response> {
    // Request to api to obtain list of available categories in system.
    return this.apiService.post(environment.baseUrl,
      ApiUrl.initiateCategory, null, category)
      .toPromise();
  }

  //#endregion
}
