module.exports = function(ngModule){

    ngModule.service('userService', function($http, $interpolate,
                                             appSettingConstant, apiUrls, urlStates){

        /*
        * Get user information.
        * */
        this.loadUsers = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.loadUsers;
            return $http.post(url, conditions);
        };

        /*
        * Get users by using specific conditions.
        * */
        this.getUsers = function(conditions){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.getUsers;
            return $http.post(url, conditions);
        };

        /*
        * Exchange access token for a profile information.
        * */
        this.getProfile = function(id){
            if (id == null)
                id = 0;

            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.getPersonalProfile;
            url = url.replace('{id}', id);
            return $http.get(url);
        };

        /*
        * Get profile state name.
        * */
        this.getProfilePage = function(id){
            var url = $interpolate('{{sref}}({profileId: {{id}}})')({sref: urlStates.user.profile.name, id: id});
            return url;
        };

        /*
        * Submit new password for a profile.
        * */
        this.changePassword = function(id, info){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.changePassword;
            url = url.replace('{id}', id);
            return $http.post(url, info);
        };

        /*
        * Use email & password to exchange with an access token.
        * */
        this.basicLogin = function(info){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.basicLogin;
            return $http.post(url, info);
        };

        /*
        * Exchange google code for local access token.
        * */
        this.fnUseGoogleLogin = function(info){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.googleLogin;
            return $http.post(url, info);
        };

        /*
        * Exchange facebook code for local access token.
        * */
        this.fnUseFacebookLogin = function (info) {
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.facebookLogin;
            return $http.post(url, info);
        };

        /*
        * Upload profile avatar to server.
        * */
        this.uploadProfileAvatar = function(avatar){
            var url = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.uploadProfileImage;

            // Initialize form data to upload image to server.
            var formData = new FormData();
            formData.append('image', avatar);

            // Add multipart/form-data to request headers.
            var options = {
                headers: {
                    'Content-Type': undefined
                }
            };

            return $http.post(url, formData, options);
        };

        /*
        * Use specific information to register an account.
        * */
        this.basicRegister = function(info){
            var fullUrl = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.basicRegister;
            return $http.post(fullUrl, info);
        };

        /*
        * Use specific information to search for user and change his/her status.
        * */
        this.editUserStatus = function(id, status){
            var fullUrl = appSettingConstant.endPoint.apiService + '/' + apiUrls.user.editUserStatus;
            var info = {
                userId: id,
                status: status
            };

            return $http.put(fullUrl, info);
        };
    });

};