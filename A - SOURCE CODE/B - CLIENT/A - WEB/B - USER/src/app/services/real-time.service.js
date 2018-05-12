module.exports = function (ngModule) {
    ngModule.service('realTimeService', function (appSettingConstant) {

        //#region Properties

        // List of hubs.
        this._hubs = {};

        //#endregion

        //#region Methods

        /*
        * Add hub to a list.
        * */
        this.addHub = function (hubName, queryString, options) {

            if (this._hubs[hubName])
                return this.getHub(hubName);

            // Get raw hub end-point.
            let hubEndPoint = this.getHubEndPoint(hubName);

            // Add connection to hashset.
            this._hubs[hubName] = {
                options: options
            };

            if (queryString) {
                let keys = Object.keys(queryString);
                let parameters = keys.map(function (key) {
                    return '' + key + '=' + queryString[key];
                });

                hubEndPoint += '?' + parameters.join('&');
                this._hubs['parameter'] = parameters;
            }

            // Initialize hub connection.
            let hubConnection = new signalR.HubConnection(hubEndPoint);
            this._hubs[hubName] = hubConnection;

            return hubConnection;
        };

        /*
        * Get hub from registered list by search for its name.
        * */
        this.getHub = function (hubName) {
            // Get hub options.
            let options = this._hubs[hubName];
            return options.hubConnection;
        };

        /*
        * Get hub options by using hub name.
        * */
        this.getHubOptions = function (hubName) {
            return this._hubs[hubName];
        };

        /*
        * Get full url of hub end-point.
        * */
        this.getHubEndPoint = function (hubName) {
            return appSettingConstant.endPoint.hubService + '/' + hubName;
        }
        //#endregion
    });
};