import {CommentsDetailsSortProperty} from "../../enumerations/order/comments-detail-sort-property";
import {SortDirection} from "../../enumerations/sort-direction";
import {Pagination} from "../../models/pagination";
import {Range} from "../../models/range";

export class SearchCommentsDetailsViewModel{

  //#region Properties

  // Id of comment.
  public id: number;

  // Index of comment owner.
  public ownerIndex: number;

  // Index of post which comment belongs to.
  public postIndex: number;

  // Time range when the comment had been created.
  public createdTime: Range<number>;

  // Time range when them comment was lastly modified.
  public lastModifiedTime: Range<number>;

  // Property which should be used for sorting.
  public sort: CommentsDetailsSortProperty;

  // Whether list should be sorted ascending or descending.
  public direction: SortDirection;

  // Pagination of comments search.
  public pagination: Pagination;

  //#endregion
}
