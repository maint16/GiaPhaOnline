module.exports = function(ngModule){
    ngModule.service('postService', function($http, appSettings, apiUrls){

        //#region Methods

        /*
        * Get posts list by using specific conditions.
        * */
        this.getPosts = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.post.getPosts;
            return $http.post(url, conditions);
        };

        //#endregion
    });
};