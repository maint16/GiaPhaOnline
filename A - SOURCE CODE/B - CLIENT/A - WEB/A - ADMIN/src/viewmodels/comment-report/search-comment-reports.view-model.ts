import {Range} from "../../models/range";
import {Pagination} from "../../models/pagination";
import {TextSearch} from "../../models/text-search";
import {CommentReportSortProperty} from "../../enumerations/order/comment-report-sort-property";
import {SortDirection} from "../../enumerations/sort-direction";
import {TextSearchMode} from "../../enumerations/text-search-mode";

export class SearchCommentReportsViewModel {

  //#region Properties

  // Index of comment report.
  public id: number;

  // Index of comment.
  public commentIndex: number;

  // Index of comment owner.
  public ownerIndex: number;

  // Index of comment reporter.
  public reporterIndex: number;

  // Body of comment.
  public body: TextSearch;

  // Reason of comment.
  public reason: TextSearch;

  // Property which should be used for results sorting.
  public sort: CommentReportSortProperty;

  // Direction of sorting.
  public direction: SortDirection;

  // Time when the comment was created.
  public createdTime: Range<number>;

  // Results pagination.
  public pagination: Pagination;

  //#endregion

  //#region Constructor

  // Initiate filter with default settings.
  public constructor() {

    let commentBody = new TextSearch();
    commentBody.mode = TextSearchMode.contains;
    commentBody.value = '';
    this.body = commentBody;

    let commentReportReason = new TextSearch();
    commentReportReason.mode = TextSearchMode.contains;
    commentReportReason.value = '';
    this.reason = commentReportReason;

    this.sort = CommentReportSortProperty.Index;
    this.direction = SortDirection.Ascending;
    this.pagination = new Pagination();
  }

  //#endregion
}
