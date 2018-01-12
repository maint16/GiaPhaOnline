import {TextSearch} from "../../models/text-search";
import {Pagination} from "../../models/pagination";
import {Range} from "../../models/range";

export class SearchCommentsViewModel {

  //#region Properties

  // Id of comment.
  public id: number;

  // Index of comment owner.
  public ownerIndex: number;

  // Index of post which comment belongs to.
  public postIndex: number;

  // Comment content.
  public content: TextSearch;

  // Time range when the comment had been created.
  public created: Range<number>;

  // Time range when them comment was lastly modified.
  public lastModified: Range<number>;

  // Pagination of comments search.
  public pagination: Pagination;

  //#endregion
}
