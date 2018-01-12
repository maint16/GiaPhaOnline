import {SearchPostReportsViewModel} from "../../../viewmodels/post-report/search-post-reports.view-model";
import {Response} from "@angular/http";

export interface IPostReportService {

  //#region Methods

  /*
  * Find post reports by using specific conditions.
  * */
  getPostReports(conditions: SearchPostReportsViewModel): Promise<Response>;

  /*
  * Delete post reports by using specific conditions.
  * */
  deletePostReports(conditions: SearchPostReportsViewModel): Promise<Response>;

  //#endregion
}
