// Import profile stylesheets.
require('./profile.css');

module.exports = function(ngModule){
    // Read templates.
    var ngPersonalProfileTemplate = require('./profile.html');

    /*
    * Route configuration.
    * */
    ngModule.config(function($stateProvider, urlStates){

        // Get state configuration.
        var urlProfileState = urlStates.user.profile;
        var urlAuthorizedLayout = urlStates.authorizedLayout;

        // State registration.
        $stateProvider.state(urlProfileState.name, {
            url: urlProfileState.url,
            parent: urlAuthorizedLayout.name,
            controller: 'profileController',
            template: ngPersonalProfileTemplate,
            resolve:{
                // Profile information.
                personalProfile: function($stateParams, $state, userService){
                    var profileId = $stateParams.profileId;
                    profileId = parseInt(profileId) || null;
                    return userService.getProfile(profileId)
                        .then(function(getProfileResponse){

                            // Get result.
                            var getProfileResult = getProfileResponse.data;

                            // Result is invalid. Re-direct user to dashboard.
                            if (!getProfileResult){
                                $state.go(urlStates.dashboard.name);
                                return;
                            }

                            return getProfileResult;
                        });
                }
            }
        });
    });
};