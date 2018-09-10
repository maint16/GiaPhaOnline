import {LoadFollowingTopicViewModel} from "../../view-models/following-topic/load-following-topic.view-model";
import {SearchResult} from "../../models/search-result";
import {FollowingTopic} from "../../models/entities/following-topic";
import {IPromise} from "angular";

export interface IFollowingTopicService {

    //#region Methods

    // Load following categories.
    loadFollowingCategories(loadFollowingTopicCondition: LoadFollowingTopicViewModel): IPromise<SearchResult<FollowingTopic>>

    //#endregion
}