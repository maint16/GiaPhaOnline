import {ItemStatus} from "../../enums/item-status.enum";
import {Pagination} from "../../models/pagination";

export class LoadFollowingCategoryViewModel {

    //#region Properties

    followerIds: number[] | null = null;

    categoryIds: number[] | null = null;

    statuses: ItemStatus[] | null = null;

    pagination: Pagination;

    //#endregion

    //#region Constructor

    public constructor(){
        this.pagination = new Pagination();
    }

    //#endregion

}