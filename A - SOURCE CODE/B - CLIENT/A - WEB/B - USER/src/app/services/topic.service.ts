import {ITopicService} from "../interfaces/services/topic-service.interface";
import {LoadTopicViewModel} from "../view-models/load-topic.view-model";
import {SearchResult} from "../models/search-result";
import {Topic} from "../models/entities/topic";
import {IHttpResponse, IHttpService} from "angular";
import {AppSetting} from "../models/app-setting";

export class TopicService implements ITopicService {


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
    * Load topics using specific conditions.
    * */
    public loadTopics(loadTopicsCondition: LoadTopicViewModel): angular.IPromise<SearchResult<Topic>> {
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/topic/search`;
        return this.$http
            .post(url, loadTopicsCondition)
            .then((loadTopicsResponse: IHttpResponse<SearchResult<Topic>>) => {
                if (!loadTopicsResponse)
                    throw 'No topics group has been found';

                let loadTopicsResult = loadTopicsResponse.data;
                if (!loadTopicsResult || !loadTopicsResult.records)
                    throw 'No topics group has been found';

                return loadTopicsResult;
            });
    }

    //#endregion
}