import {Range} from "../../models/range";
import {Pagination} from "../../models/pagination";
import {CategorySortProperty} from "../../enumerations/order/category-sort-property";
import {TextSearch} from "../../models/text-search";
import {Sorting} from "../../models/sorting";

export class SearchCategoriesViewModel{

    //#region Properties

    // Index of category.
    public id: number;

    // Name of category
    public name: TextSearch;

    // Index of category creator.
    public creatorIndex: number;

    // When the category was created.
    public createdTime: Range<number>;

    // When the category was lastly modified.
    public lastModifiedTime: Range<number>;

    // Results pagination.
    public pagination: Pagination;

    // Property and direction of sorting.
    public sorting: Sorting<CategorySortProperty>;

    //#endregion

    //#region Constructor

    // Initiate view model with properties.
    public constructor(){
        this.name = new TextSearch();
        this.creatorIndex = null;
        this.createdTime = new Range<number>();
        this.lastModifiedTime = new Range<number>();
        this.pagination = new Pagination();
        this.sorting = new Sorting<CategorySortProperty>();
    }

    //#endregion
}
