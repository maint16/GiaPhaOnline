module.exports = function(ngModule){
    ngModule.service('pushNotificationService', function(appSettingConstant, apiUrls, $http){

        //#region Methods

        /*
        * Add device to push notification service.
        * */
        this.addDevice = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.pushNotification.addDevice;
            return $http.post(url, conditions);
        };

        //#endregion
    });
};