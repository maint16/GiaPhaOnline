module.exports = function (ngModule) {
    ngModule.factory('apiInterceptor',
        function ($injector,
            $q,
            authenticationService,
            appSettings) {

            return {
                /*
                * Callback which is fired when request is made.
                * */
                request: function (x) {

                    console.log(x);
                    // Turn on loading screen.
                    //blockUI.start();

                    // Find authentication token from local storage.
                    var authenticationToken = authenticationService.getAuthenticationToken();

                    // As authentication token is found. Attach it into the request.
                    if (authenticationToken)
                        x.headers.Authorization = 'Bearer ' + authenticationToken;
                    console.log(x);
                    return x;
                },

                /*
                * Callback which is fired when request is made failingly.
                * */
                requestError: function (config) {
                    return config;
                },

                /*
                * Callback which is fired when response is sent back from back-end.
                * */
                response: function (x) {
                    // Stop blockUI.
                    //blockUI.stop();

                    return x;
                },

                /*
                * Callback which is fired when response is failed.
                * */
                responseError: function (x) {
                    // Response is invalid.
                    if (!x)
                        return $q.reject(x);
                    
                    var url = x.config.url;
                    if (!url || url.indexOf('/api/') === -1)
                        return $q.reject(x);

                    // Find state.
                    var state = $injector.get('$state');
                    var urlStates = $injector.get('urlStates');

                    // Find toastr notification from injector.
                    var toastr = $injector.get('toastr');

                    // Find translate service using injector.
                    var translate = $injector.get('$translate');

                    // Find authentication service.
                    var authenticationService = $injector.get('authenticationService');

                    var szMessage = '';
                    switch (x.status) {
                        case 401:
                            var a = x;
                            var szAuthenticateError = x.headers('WWW-Authenticate');

                            // Token is invalid.
                            if (szAuthenticateError.indexOf('invalid_token')){
                                // Clear token from local storage.
                                authenticationService.clearAuthenticationToken();

                                // Redirect user to dashboard page.
                                state.go(urlStates.dashboard.name);
                            }
                            szMessage = 'Your credential is invalid.';
                            break;
                        case 500:
                            szMessage = 'Internal server error';
                            break;
                        default:
                            szMessage = 'Unknown error';
                            break;
                    }

                    if (toastr)
                        toastr.error(szMessage, 'Error');
                    else
                        console.log(szMessage);
                    return $q.reject(x);
                }
            }
        });
};