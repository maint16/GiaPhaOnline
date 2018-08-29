import {IReplyService} from "../interfaces/services/reply-service.interface";
import {SearchResult} from "../models/search-result";
import {Reply} from "../models/entities/reply";
import {IHttpResponse, IHttpService, IPromise} from "angular";
import {LoadReplyViewModel} from "../view-models/load-reply.view-model";
import {AppSetting} from "../models/app-setting";
import {AddReplyViewModel} from "../view-models/add-reply.view-model";

/* @ngInject */
export class ReplyService implements IReplyService {

    //#region Constructor

    /*
    * Initialize service with injectors.
    * */
    public constructor(public appSettingConstant: AppSetting,
                       public $http: IHttpService){

    }

    //#endregion

    //#region Methods

    /*
    * Load replies using specific conditions.
    * */
    public loadReplies(condition: LoadReplyViewModel): IPromise<SearchResult<Reply>> {
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/reply/search`;
        return this.$http
            .post(url, condition)
            .then((loadRepliesResponse: IHttpResponse<SearchResult<Reply>>) => {
                if (!loadRepliesResponse)
                    throw 'No replies has been found';

                let loadRepliesResult = loadRepliesResponse.data;
                if (!loadRepliesResult || !loadRepliesResult.records)
                    throw 'No replies has been found';

                return loadRepliesResult;
            });
    };

    /*
    * Add reply to a specific topic.
    * */
    public addReply(model: AddReplyViewModel): IPromise<Reply> {
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/reply`;
        return this.$http
            .post(url, model)
            .then((addReplyResponse: IHttpResponse<Reply>) => {
                if (!addReplyResponse || !addReplyResponse.data)
                    throw 'Failed to add reply';

                return addReplyResponse.data;
            });
    }

    //#endregion

}