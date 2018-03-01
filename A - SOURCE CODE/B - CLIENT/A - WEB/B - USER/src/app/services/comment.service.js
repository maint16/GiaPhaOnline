module.exports = function(ngModule){
    ngModule.service('commentService', function($http, appSettings, apiUrls){

        //#region Methods

        /*
        * Get comments by using specific conditions.
        * */
        this.getComments = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.comment.getComments;
            return $http.post(url, conditions);
        };

        /*
        * Load a list of comment using indexes.
        * */
        this.loadComments = function(ids){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.comment.loadComments;
            return $http.post(url, {ids: ids});
        };

        /*
        * Add a comment to a specific post.
        * */
        this.addComment = function(comment){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.comment.addComment;
            return $http.post(url, comment);
        };

        /*
        * Delete comment by using specific information.
        * */
        this.deleteComment = function(id){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.comment.deleteComment;
            url = url.replace('{id}', id);
            return $http.delete(url);
        };

        //#endregion
    });
};