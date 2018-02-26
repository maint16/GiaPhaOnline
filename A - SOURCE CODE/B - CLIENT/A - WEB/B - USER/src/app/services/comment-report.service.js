module.exports = function(ngModule){
    ngModule.service('commentReportService', function($http, appSettings, apiUrls){

        //#region Methods

        /*
        * Get comment reports by using specific conditions.
        * */
        this.getCommentReports = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.commentReports.getCommentReports;
            return $http.post(url, conditions);
        };

        /*
        * Delete a specific comment report.
        * */
        this.deleteCommentReport = function(commentId, reporterId){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.commentReports.deleteCommentReports;

            // Construct parameter on query string.
            var params = {
                commentId: commentId,
                reporterId: reporterId
            };

            return $http.delete(url, {params: params});
        };

        //#endregion

    });
};