module.exports = function(ngModule){

    ngModule.service('userService', function($http, appSettings, apiUrls){

        /*
        * Get user information.
        * */
        this.loadUsers = function(ids){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.loadUsers;
            return $http.post(url, {ids: ids});
        };

        /*
        * Get users by using specific conditions.
        * */
        this.getUsers = function(conditions){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.getUsers;
            return $http.post(url, conditions);
        };

        /*
        * Exchange access token for a profile information.
        * */
        this.getProfile = function(){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.getPersonalProfile;
            return $http.get(url);
        };

        /*
        * Use email & password to exchange with an access token.
        * */
        this.basicLogin = function(info){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.basicLogin;
            return $http.post(url, info);
        };
    });

};