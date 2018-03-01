module.exports = function(ngModule){
    ngModule.service('followCategoryService', function($http, appSettings, apiUrls){

        /*
        * Get user following categories.
        * */
        this.getFollowingCategories = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.followingCategory.getFollowingCategory;
            return $http.post(url, conditions);
        };

        /*
        * Start following category.
        * */
        this.followCategory = function(categoryId){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.followingCategory.followCategory;
            return $http.post(url, {categoryId: categoryId});
        };

        /*
        * Stop following category.
        * */
        this.unfollowCategory = function(categoryId){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.followingCategory.unfollowCategory;
            var params = {
                categoryId: categoryId
            };

            return $http.delete(url, {params: params});
        };
    });
};