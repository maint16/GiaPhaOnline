module.exports = function (ngModule) {
    ngModule.constant('appSettingConstant', {

        identityStorage: 'iConfess',

        // End-point configuration.
        endPoint: {
            // apiService: 'http://vlqy5vs38b4xkhqra.stoplight-proxy.io'
            apiService: 'http://localhost:61356',
            hubService: 'http://localhost:61356',
            // apiService: 'http://10.7.144.50:45457',
            // hubService: 'http://10.7.144.50:45457'
        },

        // Pagination configuration.
        pagination: {
            userSelector: 20,
            default: 30,
            categoryPosts: 50,
            comments: 50,
            categoriesSelector: 10,
            postNotifications: 20
        }
    });
};