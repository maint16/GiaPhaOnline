import {Inject, Injectable} from "@angular/core";
import {SearchPostsViewModel} from "../../viewmodels/post/search-posts.view-model";
import {Response} from "@angular/http";
import {IPostService} from "../../interfaces/services/api/post-service.interface";
import {IApiService} from "../../interfaces/services/api/api-service.interface";
import {environment} from "../../environments/environment";
import {ApiUrl} from "../../constants/api-url";

@Injectable()
export class ClientPostService implements IPostService {

  //#region Properties


  //#endregion

  //#region Constructor

  /*
  * Initiate instance of category service.
  * */
  public constructor(@Inject("IApiService") private apiService: IApiService) {
  }

  //#endregion

  //#region Methods

  /*
  * Find posts by using specific conditions.
  * */
  public getPosts(conditions: SearchPostsViewModel): Promise<Response> {
    // Page page should be decrease by one.
    let localConditions = Object.assign({}, conditions);
    localConditions['pagination'] = Object.assign({}, localConditions.pagination);
    localConditions.pagination.page -= 1;
    if (localConditions.pagination.page < 0)
      localConditions.pagination.page = 0;

    // Request to api to obtain list of available categories in system.
    return this.apiService.post(environment.baseUrl, ApiUrl.getPosts,
      null,
      localConditions)
      .toPromise();
  }

  /*
  * Find post details.
  * */
  public getPostDetails(index: number): Promise<Response> {
    // Request to api to obtain list of available categories in system.
    return this.apiService.get(environment.baseUrl,ApiUrl.getPostsDetails,
      null);
  }

  //#endregion
}
