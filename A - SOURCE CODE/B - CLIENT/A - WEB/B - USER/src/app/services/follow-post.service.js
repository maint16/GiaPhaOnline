module.exports = function(ngModule){
    ngModule.service('followPostService', function($http, appSettings, apiUrls){

        //#region Methods

        /*
        * Get post information which are followed by the current user.
        * */
        this.getFollowPosts = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.followPost.getFollowPost;
            return $http.post(url, conditions);
        };

        /*
        * Load following posts.
        * */
        this.loadFollowPosts = function(ids){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.followPost.loadFollowPosts;
            return $http.post(url, {ids: ids});
        };

        //#endregion
    });
};