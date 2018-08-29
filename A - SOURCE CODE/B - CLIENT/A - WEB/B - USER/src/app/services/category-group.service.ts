import {ICategoryGroupService} from "../interfaces/services/category-group-service.interface";
import {LoadCategoryGroupViewModel} from "../view-models/load-category-group.view-model";
import {SearchResult} from "../models/search-result";
import {CategoryGroup} from "../models/entities/category-group";
import {AppSetting} from "../models/app-setting";
import {IHttpResponse, IHttpService, IPromise} from "angular";

/* @ngInject */
export class CategoryGroupService implements ICategoryGroupService {

    //#region Constructor

    public constructor(public appSettingConstant: AppSetting,
                       public $http: IHttpService){

    }

    //#endregion

    //#region Methods

    /*
    * Load topics groups by using specific conditions.
    * */
    public loadCategoryGroups(loadCategoryGroupsCondition: LoadCategoryGroupViewModel): IPromise<SearchResult<CategoryGroup>> {
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/category-group/search`;
        return this.$http
            .post(url, loadCategoryGroupsCondition)
            .then((loadCategoryGroupsResponse: IHttpResponse<SearchResult<CategoryGroup>>) => {
                if (!loadCategoryGroupsResponse || !loadCategoryGroupsResponse.data)
                    throw 'No topics group has been found';

                return loadCategoryGroupsResponse.data;
            });
    }

    //#endregion
}