module.exports = (ngModule) => {
    ngModule.service('authenticationService',
        function ($window, appSettingConstant) {

            /*
            * Getting authentication token from localStorage.
            * */
            this.getAuthenticationToken = function () {
                return $window.localStorage.getItem(appSettingConstant.identityStorage);
            };

            /*
            * Initiate authentication token into local storage.
            * */
            this.initAuthenticationToken = function (accessToken) {
                $window.localStorage.setItem(appSettingConstant.identityStorage, accessToken);
            };

            /*
            * Remove authentication token from localStorage.
            * */
            this.clearAuthenticationToken = function () {
                $window.localStorage.removeItem(appSettingConstant.identityStorage);
            };
        });
};