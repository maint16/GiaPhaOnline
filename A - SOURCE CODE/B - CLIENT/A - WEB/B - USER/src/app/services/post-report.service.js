module.exports = function(ngModule){
    ngModule.service('postReportService', function($http, apiUrls, appSettingConstant){

        //#region Methods

        /*
        * Get post reports by using specific conditions.
        * */
        this.getPostReports = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.commentReports.getCommentReports;
            return $http.post(url, conditions);
        };

        /*
        * Delete post report by using specific conditions.
        * */
        this.deletePostReport = function(postId){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.postReport.deletePostReport;
            return $http.delete(url, {params :{postId: postId}});
        };

        //#endregion
    });
};