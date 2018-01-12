import {Injectable, Inject} from "@angular/core";
import {SearchPostReportsViewModel} from "../../viewmodels/post-report/search-post-reports.view-model";
import {Response} from "@angular/http";
import {IPostReportService} from "../../interfaces/services/api/post-report-service.interface";
import {IApiService} from "../../interfaces/services/api/api-service.interface";
import {ApiUrl} from "../../constants/api-url";
import {environment} from "../../environments/environment";

@Injectable()
export class ClientPostReportService implements IPostReportService {

  //#region Properties


  //#endregion

  //#region Constructor

  // Initiate service with injectors.
  public constructor(@Inject("IApiService") private apiService: IApiService) {
  }

  //#endregion

  //#region Methods

  /*
  * Find post reports by using specific conditions.
  * */
  public getPostReports(conditions: SearchPostReportsViewModel): Promise<Response> {
    return this.apiService.post(environment.baseUrl, ApiUrl.getPostReports,
      null, conditions)
      .toPromise();
  }

  /*
  * Delete post reports by using specific conditions.
  * */
  public deletePostReports(conditions: SearchPostReportsViewModel): Promise<Response> {
    return this.apiService.delete(environment.baseUrl, ApiUrl.deletePostReport,
      null, conditions).toPromise();
  }

  //#endregion
}
