module.exports = function (ngModule) {

    /*
    * Initialize service with injectors.
    * */
    ngModule.service('categoryService', function ($http, appSettingConstant, apiUrls) {

        //#region Methods

        /*
        * Get categories by using specif
        * */
        this.getCategories = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.category.getCategories;
            return $http.post(url, conditions);
        };

        //#endregion
    });
};