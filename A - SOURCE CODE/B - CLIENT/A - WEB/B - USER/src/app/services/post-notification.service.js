module.exports = function(ngModule){
    ngModule.service('postNotificationService', function(appSettings, apiUrls,
                                                         $http){

        //#region Methods

        /*
        * Search for post notification by using specific conditions.
        * */
        this.search = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.postNotification.getPostNotifications;
            return $http.post(url, conditions);
        };

        //#endregion
    });
};