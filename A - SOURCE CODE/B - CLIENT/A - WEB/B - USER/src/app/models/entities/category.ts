import {ItemStatus} from "../../enums/item-status.enum";

export class Category {

    //#region Properties

    public id: number = 0;

    public creatorId: number = 0;

    public categoryGroupId: number = 0;

    public photo: string = '';

    public status: ItemStatus = ItemStatus.disabled;

    public name: string = '';

    public createdTime: number = 0;

    public lastModifiedTime: number | null = null;

    //#endregion
}