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

        //#endregion
    });
};