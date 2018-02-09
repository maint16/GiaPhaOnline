module.exports = function (ngModule) {

    /*
    * Initialize service with injectors.
    * */
    ngModule.service('categoryService', function ($http, appSettings, apiUrls) {

        //#region Methods

        /*
        * Get categories by using specif
        * */
        this.getCategories = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.category.getCategories;
            return $http.post(url, conditions);
        };

        //#endregion
    });
};