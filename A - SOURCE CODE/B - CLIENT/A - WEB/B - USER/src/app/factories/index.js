module.exports = (ngModule) => {

    const {ApiInterceptorFactory} = require('./api-interceptor.factory.ts');
    ngModule.factory('apiInterceptor', (localStorageService, $q, $injector) => new ApiInterceptorFactory(localStorageService, $q, $injector));
    // require('./api-interceptor.factory')(ngModule);
};