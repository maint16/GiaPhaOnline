import {TopicTypeEnum} from "../../enums/topic-type.enum";

export class Topic {

    //#region Properties

    public id: number = 0;

    public ownerId: number = 0;

    public categoryId: number = 0;

    public title: string = '';

    public body: string = '';

    public type: TopicTypeEnum = TopicTypeEnum.private;

    public createdTime: number = 0;

    public lastModifiedTime: number | null = null;
    
    //#endregion

}