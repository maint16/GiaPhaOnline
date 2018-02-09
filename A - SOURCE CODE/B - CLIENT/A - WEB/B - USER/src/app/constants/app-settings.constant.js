module.exports = function (ngModule) {
    ngModule.constant('appSettings', {

        identityStorage: 'identityStorage',

        // End-point configuration.
        endPoint: {
            apiService: 'http://vlqy5vs38b4xkhqra.stoplight-proxy.io'
        },

        // Pagination configuration.
        pagination: {
            userSelector: 20,
            default: 30,
            categoryPosts: 50
        }
    });
};