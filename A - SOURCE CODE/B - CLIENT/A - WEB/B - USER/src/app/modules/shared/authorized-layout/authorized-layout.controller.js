module.exports = function (ngModule) {
    ngModule.controller('authorizedLayoutController',
        function ($scope, $transitions, uiService, profile) {

            //#region Properties

            // Resolver reflection.
            $scope.profile = profile;

            //#endregion

            //#region Methods

            /*
            * Callback which is called when component starts being initiated.
            * */
            $scope.init = function () {
                uiService.reloadWindowSize();
            };

            /*
            * Hook the transition from state to state.
            * */
            $transitions.onSuccess({}, function (transition) {
                uiService.reloadWindowSize();
            });
            //#endregion
        });
};