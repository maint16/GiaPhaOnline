module.exports = (ngModule) => {
    // Import routes.

    const {AuthorizedLayoutModule} = require('./authorized-layout');
    ngModule.config(($stateProvider) => new AuthorizedLayoutModule($stateProvider));

    require('./unauthorized-layout/unauthorized-layout.route')(ngModule);
};