module.exports = function(ngModule){
    ngModule.service('followCategoryService', function($http, appSettings, apiUrls){

        /*
        * Get user following categories.
        * */
        this.getFollowingCategories = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.followingCategory.getFollowingCategory;
            return $http.post(url, conditions);
        };

    });
};