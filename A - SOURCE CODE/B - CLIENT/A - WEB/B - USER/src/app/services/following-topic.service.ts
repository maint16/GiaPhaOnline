import {IFollowingTopicService} from "../interfaces/services/following-topic-service.interface";
import {FollowingTopic} from "../models/entities/following-topic";
import {LoadFollowingTopicViewModel} from "../view-models/following-topic/load-following-topic.view-model";
import {SearchResult} from "../models/search-result";
import {IHttpResponse, IHttpService, IPromise} from "angular";
import {AppSetting} from "../models/app-setting";

/* @ngInject */
export class FollowingTopicService implements IFollowingTopicService {

    //#region Constructors

    public constructor(public $http: IHttpService,
                       public appSettingConstant: AppSetting){

    }

    //#endregion

    //#region Methods

    // Load following categories using specific conditions.
    public loadFollowingCategories(loadFollowingTopicCondition: LoadFollowingTopicViewModel): IPromise<SearchResult<FollowingTopic>> {
        let url = `${this.appSettingConstant.apiEndPoint}/following-topic/search`;
        return this.$http
            .post(url, loadFollowingTopicCondition)
            .then((loadFollowingTopicResponse: IHttpResponse<SearchResult<FollowingTopic>>) => {
                if (!loadFollowingTopicResponse)
                    throw 'No following topic is found';

                let loadFollowingTopicResult = loadFollowingTopicResponse.data;
                if (!loadFollowingTopicResult)
                    throw 'No following topic is found';

                return loadFollowingTopicResult;
            })
    }

    //#endregion
}