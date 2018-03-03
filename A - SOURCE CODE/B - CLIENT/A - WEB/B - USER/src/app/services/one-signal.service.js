module.exports = function(ngModule){
    ngModule.service('oneSignalService', function(){

        //#region Properties

        // Whether service has been initialized or not.
        this._bHasServiceInitialized = false;

        //#endregion

        //#region Methods

        /*
        * Add one signal sdk.
        * */
        this.addSdk = function(){
            // Sdk name.
            var szSdkName = 'onesignal-jssdk';

            // Sdk has been imported before.
            if (document.getElementById(szSdkName)) {
                return;
            }

            var oneSignalSdkScript = document.createElement('script');
            oneSignalSdkScript.id = szSdkName;
            oneSignalSdkScript.src = 'https://cdn.onesignal.com/sdks/OneSignalSDK.js?onload="fnOnOneSignalLoaded"';
            oneSignalSdkScript.async = true;
            document.head.appendChild(oneSignalSdkScript);
        };

        /*
        * Initialize service with configuration.
        * */
        this.init = function(appId, promptOptions, welcomeNotification, notifyButton, autoRegister,persistNotification){

            // Service has been initialized before. Don't initialize it.
            if (this._bHasServiceInitialized)
                return;

            // Find one signal instance.
            var OneSignal = window.OneSignal || [];

            // Options to be initialized.
            var options = {
                appId: appId,
                promptOptions: promptOptions,
                welcomeNotification: welcomeNotification,
                notifyButton: notifyButton,
                autoRegister: autoRegister,
                persistNotification: persistNotification
            };

            // Initialize service.
            OneSignal.push(function() {
                OneSignal.init(options);
            });

            // Mark the service as has been initialized.
            this._bHasServiceInitialized = true;
        };

        //#endregion
    });
};