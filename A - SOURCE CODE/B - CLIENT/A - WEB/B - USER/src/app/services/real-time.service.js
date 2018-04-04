module.exports = function(ngModule){
  ngModule.service('realTimeService', function(appSettings){

      //#region Properties

      // List of hubs.
      this._hubs = {};

      //#endregion

      //#region Methods

      /*
      * Add hub to a list.
      * */
      this.addHub = function(hubName, queryString, options){

          if (this._hubs[hubName])
              return this.getHub(hubName);

          debugger;
          // Get raw hub end-point.
          var hubEndPoint = this.getHubEndPoint(hubName);

          if (queryString){
              var keys = Object.keys(queryString);
              var parameters = keys.map(function(key){
                  return '' + key + '=' + queryString[key];
              });

              hubEndPoint += '?' + parameters.join('&')
          }

          // Initialize hub connection.
          var hubConnection = new signalR.HubConnection(hubEndPoint);


          // Add connection to hashset.
          this._hubs[hubName] = {
              hubConnection: hubConnection,
              parameter:parameters,
              options: options
          };

          return hubConnection;
      };

      /*
      * Get hub from registered list by search for its name.
      * */
      this.getHub = function(hubName){
          // Get hub options.
          var options = this._hubs[hubName];
          return options.hubConnection;
      };

      /*
      * Get hub options by using hub name.
      * */
      this.getHubOptions = function(hubName){
          return this._hubs[hubName];
      };

      /*
      * Get full url of hub end-point.
      * */
      this.getHubEndPoint = function(hubName){
          return appSettings.endPoint.hubService + '/' + hubName;
      }
      //#endregion
  });
};