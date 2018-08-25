module.exports = (ngModule) => {
    // Route config.
    ngModule.config(($stateProvider) => {

        // Import constants.
        const UrlStateConstant = require('../../../constants/url-state.constant.ts').UrlStateConstant;

        // State configuration
        $stateProvider.state(UrlStateConstant.authorizedLayoutModuleName, {
            controller: 'authorizedLayoutController',
            abstract: true,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => {
                        require('./authorized-layout.scss');
                        resolve(require('./authorized-layout.html'))
                    });
                });
            }],
            resolve: {
                profile: (notificationStatusConstant, appSettingConstant,
                          userService, authenticationService) => {

                    // Get access token from storage.
                    const accessToken = authenticationService.getAuthenticationToken();

                    // No access token has been defined.
                    if (!accessToken || accessToken.length < 1)
                        return null;

                    // // Promises to be resolved.
                    // let promises = [];
                    //
                    // //#region Get profile
                    //
                    // // Get user profile promise.
                    // promises[0] = userService.getProfile(0)
                    //     .then((getProfileResponse) => {
                    //         return getProfileResponse.data;
                    //     });
                    //
                    // //#endregion
                    //
                    // return Promise.all(promises)
                    //     .then(function (promiseResponses) {
                    //
                    //         // Get profile.
                    //         return promiseResponses[0];
                    //     });
                    return null;
                },

                /*
                * Load authorized-layout controller.
                * */
                loadAuthorizedLayoutController: ($q, $ocLazyLoad) => {
                    return $q((resolve) => {
                        require.ensure([], () => {
                            // load only controller module
                            let module = angular.module('shared.authorized-layout', []);
                            require('./authorized-layout.controller')(module);
                            $ocLazyLoad.load({name: module.name});
                            resolve(module.controller);
                        })
                    });
                }
            }
        });
    });
};