module.exports = (ngModule) => {
    const {AuthorizedLayoutModule} = require('./authorized-layout');
    ngModule.config(($stateProvider) => new AuthorizedLayoutModule($stateProvider));

    const {UnauthorizedLayoutModule} = require('./unauthorized-layout');
    ngModule.config(($stateProvider) => new UnauthorizedLayoutModule($stateProvider));
};