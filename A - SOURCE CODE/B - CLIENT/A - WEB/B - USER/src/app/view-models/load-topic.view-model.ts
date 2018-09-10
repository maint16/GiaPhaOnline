import {TopicType} from "../enums/topic-type.enum";
import {ItemStatus} from "../enums/item-status.enum";
import {Pagination} from "../models/pagination";

export class LoadTopicViewModel {

    //#region Properties

    public ids: Array<number> | null = null;

    public ownerIds: Array<number> | null = null;

    public categoryIds: Array<number> | null = null;

    public types: Array<TopicType> | null = null;

    public statuses: Array<ItemStatus> | null = null;

    public pagination: Pagination | null = null;

    //#endregion

}