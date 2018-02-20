module.exports = function(ngModule){

    ngModule.service('userService', function($http, $interpolate,
                                             appSettings, apiUrls, urlStates){

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
        this.getProfile = function(id){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.getPersonalProfile;
            url = url.replace('{id}', id);
            return $http.get(url);
        };

        /*
        * Get profile state name.
        * */
        this.getProfilePage = function(id){
            var url = $interpolate('{{sref}}({profileId: {{id}}})')({sref: urlStates.user.profile.name, id: id});
            console.log(url);
            return url;
        };

        /*
        * Submit new password for a profile.
        * */
        this.changePassword = function(id, info){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.changePassword;
            url = url.replace('{id}', id);
            return $http.post(url, info);
        };

        /*
        * Use email & password to exchange with an access token.
        * */
        this.basicLogin = function(info){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.basicLogin;
            return $http.post(url, info);
        };

        /*
        * Exchange google code for local access token.
        * */
        this.fnUseGoogleLogin = function(info){
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.googleLogin;
            return $http.post(url, info);
        };

        /*
        * Exchange facebook code for local access token.
        * */
        this.fnUseFacebookLogin = function (info) {
            var url = appSettings.endPoint.apiService + '/' + apiUrls.user.facebookLogin;
            return $http.post(url, info);
        }
    });

};