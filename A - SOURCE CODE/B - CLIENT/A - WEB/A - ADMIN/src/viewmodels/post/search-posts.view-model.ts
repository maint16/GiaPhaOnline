import {TextSearch} from "../../models/text-search";
import {Range} from "../../models/range";
import {Pagination} from "../../models/pagination";

export class SearchPostsViewModel{

  //#region Properties

    // Id of post
    public id: number;

    // Owner index of post.
    public ownerIndex: number;

    // Category index of post.
    public categoryIndex: number;

    // Title of post
    public title: TextSearch;

    // Body of post
    public body: TextSearch;

    // Time when post is created.
    public created: Range<number>;

    // Time when post was lastly modified.
    public lastModified: Range<number>;

    // Pagination of records.
    public pagination: Pagination;

    //#endregion
}
