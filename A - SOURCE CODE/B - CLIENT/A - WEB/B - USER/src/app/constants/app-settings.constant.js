module.exports = function (ngModule) {
    ngModule.constant('appSettings', {

        identityStorage: 'iConfess',

        // End-point configuration.
        endPoint: {
            // apiService: 'http://vlqy5vs38b4xkhqra.stoplight-proxy.io'
            apiService: 'http://localhost:61356'
            // apiService: 'http://10.7.144.76:45455'
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