import {Range} from "../../models/range";
import {Pagination} from "../../models/pagination";

export class LoadFollowingTopicViewModel {

    //#region Properties

    public followerIds: number[] | null;

    public topicIds: number[] | null;

    public createdTime: Range<number | null, number | null> | null;

    public pagination: Pagination;

    //#endregion

    //#region Constructor

    // Initialize view model with settings.
    public constructor(){
        let pagination = new Pagination();
        pagination.page = 1;
        pagination.records = 1;
        this.pagination = pagination;
    }

    //#endregion
}