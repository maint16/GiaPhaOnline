module.exports = function(ngModule){

    ngModule.service('oAuthService', function($interpolate ){

        //#region Methods

        /*
        * Add facebook SDK.
        * */
        this.addFacebookSdk = function(){

            // Sdk name.
            var szSdkName = 'facebook-jssdk';

            // Sdk has been imported before.
            if (document.getElementById(szSdkName)) {
                return;
            }

            var facebookSdkScript = document.createElement('script');
            facebookSdkScript.id = szSdkName;
            facebookSdkScript.src = 'https://connect.facebook.net/en_US/sdk.js';
            document.head.appendChild(facebookSdkScript);
        };

        /*
        * Add Google SDK.
        * */
        this.addGoogleSdk = function(szOnloadFunctionName){

            var szSdkName = 'google-jssdk';

            // Sdk has been imported before.
            if (document.getElementById(szSdkName)) {
                return;
            }

            var data = {
                function:{
                    onLoad: szOnloadFunctionName
                }
            };

            var apiGoogleScript = document.createElement('script');
            apiGoogleScript.id = szSdkName;
            apiGoogleScript.type = 'text/javascript';
            apiGoogleScript.async = true;
            apiGoogleScript.src = $interpolate('https://apis.google.com/js/platform.js?onload={{function.onLoad}}')(data);
            document.head.appendChild(apiGoogleScript);
        };

        //#endregion
    });
};