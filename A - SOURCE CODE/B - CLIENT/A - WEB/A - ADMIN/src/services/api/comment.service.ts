import {Injectable, Inject} from "@angular/core";
import {SearchCommentsViewModel} from "../../viewmodels/comment/search-comments.view-model";
import {SearchCommentsDetailsViewModel} from "../../viewmodels/comment/search-comments-details.view-model";
import {ICommentService} from "../../interfaces/services/api/comment-service.interface";
import {Response} from "@angular/http";
import {IApiService} from "../../interfaces/services/api/api-service.interface";
import {environment} from "../../environments/environment";
import {ApiUrl} from "../../constants/api-url";

@Injectable()
export class ClientCommentService implements ICommentService {

  //#region Properties


  //#endregion

  //#region Constructor

  // Initiate service with injectors.
  public constructor(@Inject("IApiService") public apiService: IApiService) {
  }

  //#endregion

  //#region Methods

  /*
  * Search for comments by using specific conditions.
  * */
  public getComments(conditions: SearchCommentsViewModel): Promise<Response> {
    return this.apiService.post(environment.baseUrl, ApiUrl.getComments,
      null, conditions)
      .toPromise();
  }

  /*
  * Search for a specific comment's detail.
  * */
  public getCommentDetails(conditions: SearchCommentsDetailsViewModel): Promise<Response> {
    return this.apiService.post(environment.baseUrl, ApiUrl.getCommentsDetails,
      null, conditions)
      .toPromise();
  }

  //#endregion
}
