import {TopicType} from "../../enums/topic-type";

export class Topic {

    //#region Properties

    public id: number = 0;

    public ownerId: number = 0;

    public categoryId: number = 0;

    public title: string = '';

    public body: string = '';

    public type: TopicType = TopicType.private;

    public createdTime: number = 0;

    public lastModifiedTime: number | null = null;
    
    //#endregion

}