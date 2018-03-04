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
                profile: function (notificationStatusConstant, appSettings,
                                   userService, authenticationService, postNotificationService) {

                    // Get access token from storage.
                    var accessToken = authenticationService.getAuthenticationToken();

                    // No access token has been defined.
                    if (!accessToken || accessToken.length < 1)
                        return null;

                    // Promises to be resolved.
                    var promises = [];

                    //#region Get profile

                    // Get user profile promise.
                    promises[0] = userService.getProfile()
                        .then(function (getProfileResponse) {
                            return getProfileResponse.data;
                        });

                    //#endregion

                    //#region Get post notifications

                    // Get post notification promise.
                    var getPostNotificationCondition = {
                        statuses: [notificationStatusConstant.unseen],
                        pagination: {
                            page: 1,
                            records: appSettings.pagination.postNotifications
                        }
                    };

                    // Add promise to queue.
                    promises[1] = postNotificationService.search(getPostNotificationCondition)
                        .then(function (getPostNotificationResponse) {
                            return getPostNotificationResponse.data;
                        });

                    //#endregion

                    return Promise.all(promises)
                        .then(function (promiseResponses) {

                            // Get profile.
                            var profile = promiseResponses[0];
                            var getPostNotificationResult = promiseResponses[1];

                            // Merge profile information.
                            profile['getPostNotificationsResult'] = getPostNotificationResult;

                            return profile;
                        });
                }
            }
        });
    });
};