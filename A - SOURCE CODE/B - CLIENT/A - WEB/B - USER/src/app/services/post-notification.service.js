module.exports = function(ngModule){
    ngModule.service('postNotificationService', function(appSettingConstant, apiUrls,
                                                         $http){

        //#region Methods

        /*
        * Search for post notification by using specific conditions.
        * */
        this.search = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.postNotification.getPostNotifications;
            return $http.post(url, conditions);
        };

        //#endregion
    });
};