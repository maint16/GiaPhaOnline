module.exports = function(ngModule){
    ngModule.service('followPostService', function($http, appSettingConstant, apiUrls){

        //#region Methods

        /*
        * Get post information which are followed by the current user.
        * */
        this.getFollowPosts = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.followPost.getFollowPost;
            return $http.post(url, conditions);
        };

        /*
        * Load following posts.
        * */
        this.loadFollowPosts = function(ids){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.followPost.loadFollowPosts;
            return $http.post(url, {ids: ids});
        };

        /*
        * Start following a post.
        * */
        this.followPost = function (id) {
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.followPost.followPost;
            var body = {
                postId: id
            };

            return $http.post(url, body);
        };

        /*
        * Stop following a post.
        * */
        this.unfollowPost = function(id){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.followPost.unfollowPost;
            var query = {
                postId: id
            };

            return $http.delete(url, {params: query});
        };

        //#endregion
    });
};