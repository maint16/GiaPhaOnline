module.exports = function (ngModule) {

    // Module html template import.
    var ngModuleHtmlTemplate = require('./user-management.html');

    ngModule.config(function ($stateProvider, urlStates) {

        var urlStateControlPanel = urlStates.controlPanel;
        var urlStateControlPanelMasterLayout = urlStateControlPanel.masterLayout;
        var urlUserManagement = urlStateControlPanel.userManagement;

        $stateProvider.state(urlUserManagement.name, {
            url: urlUserManagement.url,
            controller: 'userManagementController',
            parent: urlStateControlPanelMasterLayout.name,
            template: ngModuleHtmlTemplate
        });
    });
};