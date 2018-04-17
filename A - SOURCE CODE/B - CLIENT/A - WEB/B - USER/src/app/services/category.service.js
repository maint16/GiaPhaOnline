module.exports = function (ngModule) {

    /*
    * Initialize service with injectors.
    * */
    ngModule.service('categoryService', function ($http,
                                                  appSettingConstant, apiUrls, itemStatusConstant) {

        //#region Methods

        /*
        * Get categories by using specif
        * */
        this.getCategories = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.category.getCategories;
            return $http.post(url, conditions);
        };

        /*
        * Find and update a category by searching for its index.
        * */
        this.editCategory = function(id, info){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.category.editCategory;
            url = url.replace('{id}', id);
            return $http.put(url, info);
        };

        /*
        * Delete a category by using its index.
        * */
        this.deleteCategory = function(id){
            var info = {
                status: itemStatusConstant.deleted
            };

            return this.editCategory(id, info);
        };

        /*
        * Add category into system.
        * */
        this.addCategory = function(info){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.category.addCategory;
            return $http.post(url, info);
        };

        //#endregion
    });
};