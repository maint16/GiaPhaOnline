module.exports = function(ngModule){
    ngModule.service('postReportService', function($http, apiUrls, appSettings){

        //#region Methods

        /*
        * Get post reports by using specific conditions.
        * */
        this.getPostReports = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.commentReports.getCommentReports;
            return $http.post(url, conditions);
        };

        /*
        * Delete post report by using specific conditions.
        * */
        this.deletePostReport = function(postId){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.postReport.deletePostReport;
            return $http.delete(url, {params :{postId: postId}});
        };

        //#endregion
    });
};