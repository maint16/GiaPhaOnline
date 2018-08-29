import {Pagination} from "../models/pagination";

export class LoadCategoryViewModel {

    //#region Properties

    // List of category group indexes.
    public categoryGroupIds: Array<number> | null;

    // Pagination.
    public pagination: Pagination | null;

    //#endregion

    //#region Constructor

    public constructor(){
        this.categoryGroupIds = null;
        this.pagination = new Pagination();
    }

    //#endregion
}