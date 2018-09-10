import {ItemStatus} from "../../enums/item-status.enum";

export class FollowingTopic {

    //#region Properties

    public followerId: number = 0;

    public topicId: number = 0;

    public status: ItemStatus = ItemStatus.available;

    public createdTime: number = 0;

    //#endregion

}