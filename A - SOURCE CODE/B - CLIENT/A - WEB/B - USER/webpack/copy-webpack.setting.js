// Import libraries.
var path = require('path');

// Import settings.
var settings = require('./webpack-setting');

exports = module.exports = {

    /*
    * Get copy-webpack-plugin configuration.
    * */
    get: function (root) {
        // Find application path.
        var app = settings.paths.getApplication(root);
        var src = settings.paths.getSource(root);
        var dist = settings.paths.getDist(root);

        return [
            {
                from: path.resolve(app, 'assets'),
                to: path.resolve(dist, 'assets')
            },
            {
                from: path.resolve(src, 'favicon.ico'),
                to: path.resolve(dist, 'favicon.ico')
            },
            {
                from: path.resolve(app, 'firebase-messaging-sw.js'),
                to: path.resolve(dist, 'firebase-messaging-sw.js')
            }]
    }
};

return module.exports;