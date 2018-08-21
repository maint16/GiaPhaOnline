module.exports = (ngModule) => {
    ngModule.factory('apiInterceptor',
        ($injector,
         $q,
         authenticationService,
         appSettingConstant) => {

            return {
                /*
                * Callback which is fired when request is made.
                * */
                request: (x) => {
                    // Turn on loading screen.
                    //blockUI.start();

                    // Find authentication token from local storage.
                    const authenticationToken = authenticationService.getAuthenticationToken();

                    // As authentication token is found. Attach it into the request.
                    if (authenticationToken)
                        x.headers.Authorization = 'Bearer ' + authenticationToken;

                    return x;
                },

                /*
                * Callback which is fired when request is made failingly.
                * */
                requestError: (config) => {
                    return config;
                },

                /*
                * Callback which is fired when response is sent back from back-end.
                * */
                response: (x) => {
                    // Stop blockUI.
                    //blockUI.stop();

                    return x;
                },

                /*
                * Callback which is fired when response is failed.
                * */
                responseError: (x) => {
                    // Response is invalid.
                    if (!x)
                        return $q.reject(x);

                    const url = x.config.url;
                    if (!url || url.indexOf('/api/') === -1)
                        return $q.reject(x);

                    // Find state.
                    const state = $injector.get('$state');
                    const urlStates = $injector.get('urlStates');

                    // Find toastr notification from injector.
                    const toastr = $injector.get('toastr');

                    // Find translate service using injector.
                    const translate = $injector.get('$translate');

                    // Find authentication service.
                    const authenticationService = $injector.get('authenticationService');

                    let szMessage = '';
                    switch (x.status) {
                        case 401:
                            const szAuthenticateError = x.headers('WWW-Authenticate');

                            // Token is invalid.
                            if (szAuthenticateError.indexOf('invalid_token')) {
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