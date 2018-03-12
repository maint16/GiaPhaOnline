/*
* Module exports.
* */
module.exports = function (ngModule) {
    ngModule.controller('controlPanelMasterLayoutController', function ($scope, toastr, $ngConfirm, $translate,
                                                             $timeout, $state,
                                                             appSettings, urlStates,
                                                             profile,
                                                             categoryService, postCategorizationService, followCategoryService) {

        //#region Properties

        // Current state name.
        $scope.$state = $state;

        // State reflection.
        $scope.urlStateControlPanel = null;

        //#endregion

        //#region Methods

        /*
        * Called when content initialized.
        * */
        $scope.fnContentInit = function(){

            // State reflection.
            var urlStateControlPanel = urlStates.controlPanel;
            $scope.urlStateControlPanel = urlStateControlPanel;

            // Check url.
            var szCurrentStateName = $state.current.name;

            // List of valid states.
            var validStateNames = [urlStateControlPanel.userManagement.name, urlStateControlPanel.categoryManagement.name];

            // State is not valid.
            if (validStateNames.indexOf(szCurrentStateName) < 0){
                // Access to user management.
                $state.go(urlStateControlPanel.userManagement.name);
                return;
            }
            
            $scope.$applyAsync();
        };

        /*
        * Go to state.
        * */
        $scope.fnGoToState = function(szName){

            // Name is the current one. Skip getting to it.
            if (szName === $state.current.name)
                return;

            $state.go(szName);
        };

        //#endregion
    });
};