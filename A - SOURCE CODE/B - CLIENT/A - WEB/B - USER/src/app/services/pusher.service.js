module.exports = function(ngModule){

    /*
    * Initialize pusher service with configurations.
    * */
    ngModule.service('pusherService', function(){

        //#region Properties

        // Instance of pusher.
        this._pusher = null;

        //#endregion

        //#region Methods

        /*
        * Initialize pusher instance to subscribe to real-time channels.
        * */
        this.init = function(appKey, appCluster, encrypted, authorizer, bIsOverwritten){

            // Instance shouldn't be overwritten.
            if (!bIsOverwritten && this._pusher){
                return this._pusher;
            }

            // Pusher options.
            var options = {};
            options['cluster'] = appCluster;
            options['encrypted'] = encrypted == null ? false: encrypted;

            // Authorizer function is defined.
            if (authorizer)
                options['authorizer'] = authorizer;

            // Initialize pusher.
            this._pusher = new Pusher(appKey, options);
            return this._pusher;
        };

        /*
        * Subscribe to a specific channel.
        * */
        this.subscribeChannel = function(szChannelName){
            // Channel name is empty.
            if (!szChannelName || szChannelName.length < 1)
                throw 'Channel name is invalid';

            this._pusher.subscribe(szChannelName);
            return this._pusher;
        };

        /*
        * Listen to a specific event.
        * */
        this.listenToEvent = function(szEventName, callbackFunction){
            // Event name is empty.
            if (!szEventName || szEventName.length < 1)
                throw 'Event name is empty';

            // Callback function is undefined.
            if (!callbackFunction)
                throw 'Callback function is invalid';

            this._pusher.bind(szEventName, callbackFunction);
            return this._pusher;
        };

        //#endregion
    });
};