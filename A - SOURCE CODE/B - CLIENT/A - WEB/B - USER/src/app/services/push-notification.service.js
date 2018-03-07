module.exports = function(ngModule){
    ngModule.service('pushNotificationService', function(appSettings, apiUrls, $http){

        //#region Methods

        /*
        * Add device to push notification service.
        * */
        this.addDevice = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.pushNotification.addDevice;
            return $http.post(url, conditions);
        };

        //#endregion
    });
};