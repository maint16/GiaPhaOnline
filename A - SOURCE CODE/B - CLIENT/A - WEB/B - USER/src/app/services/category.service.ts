import {ICategoryService} from "../interfaces/services/category-service.interface";
import {LoadCategoryViewModel} from "../view-models/load-category.view-model";
import {SearchResult} from "../models/search-result";
import {Category} from "../models/entities/category";
import {IHttpResponse, IHttpService, IPromise} from "angular";

/* @ngInject */
export class CategoryService implements ICategoryService {

    //#region Constructors

    public constructor(public $http: IHttpService,
                       public appSettingConstant: any){

    }

    //#endregion

    //#region Methods

    /*
    * Load categories by using specific conditions.
    * */
    public loadCategories(loadCategoriesCondition: LoadCategoryViewModel): IPromise<SearchResult<Category>> {

        // Build url.
        let fullUrl = `${this.appSettingConstant.apiEndPoint}/api/category/search`;

        return this.$http
            .post(fullUrl, loadCategoriesCondition)
            .then((loadCategoriesResponse: IHttpResponse<SearchResult<Category>>) => {
                if (!loadCategoriesResponse || !loadCategoriesResponse.data)
                    throw 'No topics has been found';

                return loadCategoriesResponse.data;
            })
    }

    //#endregion
}