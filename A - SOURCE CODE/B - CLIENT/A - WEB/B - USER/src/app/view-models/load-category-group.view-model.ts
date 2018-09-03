import {Pagination} from "../models/pagination";

export class LoadCategoryGroupViewModel {

    //#region Properties

    /*
    * List of topics group indexes.
    * */
    public ids: Array<number> | null = null;

    /*
    * Pagination information.
    * */
    public pagination: Pagination | null = null;

    //#endregion
}