import {Pagination} from "../../models/pagination";
import {SortDirection} from "../../enumerations/sort-direction";
import {PostReportSortProperty} from "../../enumerations/order/post-report-sort-property";
import {Range} from "../../models/range";

export class SearchPostReportsViewModel {

  //#region Properties

  /*
  * Id of report.
  * */
  public id: number;

  // Id of post.
  public postIndex: number;

  // Post owner index.
  public postOwnerIndex: number;

  // Post reporter index.
  public postReporterIndex: number;

  // Time range when post report was created.
  public created: Range<number>;

  // Whether record should be sorted ascending or descending.
  public direction: SortDirection;

  // Post report sort property.
  public sort: PostReportSortProperty;

  // Records pagination.
  public pagination: Pagination;

  //#endregion
}
