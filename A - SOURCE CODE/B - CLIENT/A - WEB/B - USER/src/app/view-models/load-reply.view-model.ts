import {Pagination} from "../models/pagination";

export class LoadReplyViewModel {

    //#region Properties

    /*
    * Reply indexes.
    * */
    public ids: Array<number> | null = null;

    /*
    * Reply owner indexes.
    * */
    public ownerIds: Array<number> | null = null;

    /*
    * Topic indexes.
    * */
    public topicIds: Array<number> | null = null;

    /*
    * Pagination information.
    * */
    public pagination: Pagination = new Pagination();

    //endregion

}