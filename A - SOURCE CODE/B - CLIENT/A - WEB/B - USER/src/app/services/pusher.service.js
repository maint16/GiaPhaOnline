module.exports = function(ngModule){

    /*
    * Initialize pusher service with configurations.
    * */
    ngModule.service('pusherService', function(appSettings, apiUrls,
                                               $http){

        //#region Properties

        // Instance of pusher.
        this._socket = null;

        //#endregion

        //#region Methods

        /*
        * Set socket instance.
        * */
        this.setInstance = function(socket){
            this._socket = socket;
        };

        /*
        * Get pusher instance.
        * */
        this.getInstance = function(){
            return this._socket;
        };

        /*
        * Using specific information to authorize
        * */
        this.authorizeRealTimeChannel = function(channelName, deviceId){
            // Initialize request options.
            var options = {
                socketId: deviceId,
                channelName: channelName
            };

            // Build url to make request to.
            var url = appSettings.endPoint.apiService + '/' + apiUrls.realtime.authorizePusher;
            return $http.post(url, options);
        };

        //#endregion
    });
};