import {TopicType} from "../../enums/topic-type.enum";
import {ItemStatus} from "../../enums/item-status.enum";

export class FollowingTopicViewModel {

    //#region Properties

    public id: number = 0;

    public ownerId: number = 0;

    public categoryId: number = 0;

    public title: string = '';

    public type: TopicType = TopicType.private;

    public status: ItemStatus = ItemStatus.available;

    public createdTime: number = 0;

    public lastModifiedTime: number | null = null;

    //#endregion

}