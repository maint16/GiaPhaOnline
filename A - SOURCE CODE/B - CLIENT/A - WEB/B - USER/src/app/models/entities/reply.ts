import {ItemStatus} from "../../enums/item-status.enum";

export class Reply {

    //#region Properties

    public id: number = 0;

    public ownerId: number = 0;

    public topicId: number = 0;

    public content: string = '';

    public status: ItemStatus = ItemStatus.disabled;

    public createdTime: number = 0;

    public lastModifiedTime: number | null = 0;

    //#endregion

}