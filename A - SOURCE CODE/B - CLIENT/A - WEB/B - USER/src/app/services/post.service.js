module.exports = function(ngModule){
    ngModule.service('postService', function($http, appSettingConstant, apiUrls){

        //#region Methods

        /*
        * Get posts list by using specific conditions.
        * */
        this.getPosts = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.post.getPosts;
            return $http.post(url, conditions);
        };

        /*
        * Add post to system.
        * */
        this.addPost = function(condition){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.post.addPost;
            return $http.post(url, condition);
        };

        /*
        * Search for posts by using indexes.
        * */
        this.loadPosts = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.post.loadPosts;
            return $http.post(url, conditions);
        };

        //#endregion
    });
};