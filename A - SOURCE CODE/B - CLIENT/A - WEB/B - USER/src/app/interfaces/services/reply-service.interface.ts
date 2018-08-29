import {SearchResult} from "../../models/search-result";
import {Reply} from "../../models/entities/reply";
import {IPromise} from "angular";
import {LoadReplyViewModel} from "../../view-models/load-reply.view-model";
import {AddReplyViewModel} from "../../view-models/add-reply.view-model";

export interface IReplyService {

    //#region Methods

    /*
    * Load replies by using specific conditions.
    * */
    loadReplies(condition: LoadReplyViewModel): IPromise<SearchResult<Reply>>;

    /*
    * Add reply to a specific topic.
    * */
    addReply(model: AddReplyViewModel): IPromise<Reply>;

    //#endregion

}