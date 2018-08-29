import {LoadTopicViewModel} from "../../view-models/load-topic.view-model";
import {SearchResult} from "../../models/search-result";
import {Topic} from "../../models/entities/topic";
import {IPromise} from "angular";

export interface ITopicService {

    //#region Methods

    /*
    * Load topics using specific conditions.
    * */
    loadTopics(loadTopicsCondition: LoadTopicViewModel): IPromise<SearchResult<Topic>>;

    //#endregion

}