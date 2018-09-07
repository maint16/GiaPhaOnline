const path = require('path');
const moduleRuleOption = require('./webpack/rule.option');
const modulePluginOption = require('./webpack/plugin.option');

module.exports = (env, argv) => {
    // Import webpack settings.
    const webpackOption = require('./webpack/option');

    // True if built is set to production mode.
    // False if built is set to development mode.
    let mode = argv.mode;
    if (!mode)
        mode = 'development';
    const bProductionMode = 'production' === mode.toLowerCase();

    // Build path options.
    let paths = {
        root: __dirname,
        source: webpackOption.paths.getSource(__dirname),
        app: webpackOption.paths.getApplication(__dirname),
        dist: webpackOption.paths.getDist(__dirname)
    };

    /*
    * Module export.
    * */
    return {
        context: webpackOption.paths.getSource(__dirname),
        entry: {
            'app': path.resolve(paths.app, 'app.js')
        },
        devtool: "source-map",
        optimization: {
            runtimeChunk: 'single',
            splitChunks: {
                chunks: 'all',
                cacheGroups: {
                    default: {
                        enforce: true,
                        priority: 1
                    },
                    vendors: {
                        test: /[\\/]node_modules[\\/]/,
                        priority: 2,
                        name: 'vendors',
                        enforce: true,
                        chunks: 'async'
                    }
                }
            }
        },
        module: {
            rules: moduleRuleOption.get()
        },
        plugins: modulePluginOption.get(bProductionMode, paths),
        output: {
            path: path.resolve(paths.dist),
            filename: '[name].[hash].js'
        },
        resolve: {
            // Add `.ts` and `.tsx` as a resolvable extension.
            extensions: [".ts", ".tsx", ".js"]
        }

    };
};