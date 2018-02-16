module.exports = function (ngModule) {

    // Html template import.
    var ngModuleHtmlTemplate = require('./authorized-layout.html');

    // Route config.
    ngModule.config(function ($stateProvider, urlStates) {

        // Constants reflection.
        var urlAuthorizedLayoutState = urlStates.authorizedLayout;

        // State configuration
        $stateProvider.state(urlAuthorizedLayoutState.name, {
            controller: 'authorizedLayoutController',
            abstract: true,
            template: ngModuleHtmlTemplate,
            resolve: {
                profile: function (userService, authenticationService) {

                    // Get access token from storage.
                    var accessToken = authenticationService.getAuthenticationToken();

                    // No access token has been defined.
                    if (!accessToken || accessToken.length < 1)
                        return null;

                    // Get profile from api.
                    return userService.getProfile()
                        .then(
                            function success(getProfileResponse) {
                                var getProfileResult = getProfileResponse.data;
                                if (!getProfileResult)
                                    return null;

                                return getProfileResult;
                            },
                            function error(getProfileResponse){
                                return null;
                            });
                }
            }
        });
    });
};