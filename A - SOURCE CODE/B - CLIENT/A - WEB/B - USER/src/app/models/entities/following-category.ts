import {ItemStatus} from "../../enums/item-status.enum";

export class FollowingCategory {

    //#region Properties

    public followerId: number = 0;

    public categoryId: number = 0;

    public status: ItemStatus = ItemStatus.available;

    public createdTime: number = 0;

    //#endregion

}