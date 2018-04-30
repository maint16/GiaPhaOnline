exports = module.exports = {

    /*
    * Get copy-webpack-plugin configuration.
    * */
    get: function () {

        // List of rules.
        var rules = [];

        //#region Parameter resolvers

        // JQuery.
        rules.push({
            test: require.resolve('jquery'),
            use: [
                {
                    loader: 'expose-loader',
                    options: 'jQuery'
                }, {
                    loader: 'expose-loader',
                    options: '$'
                }
            ]
        });

        // RxJS.
        rules.push({
            test: require.resolve('rxjs/bundles/rxjs.umd'),
            use: [
                {
                    loader: 'expose-loader',
                    options: 'Rx'
                }
            ]
        });

        // Bluebird promise.
        rules.push({
            test: require.resolve('bluebird'),
            use: [
                {
                    loader: 'expose-loader',
                    options: 'Promise'
                }
            ]
        });

        // Pusher
        rules.push({
            test: require.resolve('pusher-js'),
            use:[
                {
                    loader: 'expose-loader',
                    options: 'Pusher'
                }
            ]
        });

        // SignalR.
        rules.push({
            test: require.resolve('@aspnet/signalr/dist/cjs'),
            use:[
                {
                    loader: 'expose-loader',
                    options: 'signalR'
                }
            ]
        });

        //#endregion

        //#region Loaders

        // Css loader.
        rules.push({
            test: /\.css$/,
            use: ['style-loader', 'css-loader']
        });

        // Assets loader.
        rules.push({
            test: /\.(png|jpg|gif|woff|woff2|eot|ttf|svg)$/,
            use: [
                {
                    loader: 'url-loader',
                    options: {
                        limit: 8192
                    }
                }
            ]
        });

        // Html loader.
        rules.push({
            test: /\.html$/, // Only .html files
            loader: 'html-loader' // Run html loader
        });

        //#endregion

        return rules;
    }
};

return module.exports;