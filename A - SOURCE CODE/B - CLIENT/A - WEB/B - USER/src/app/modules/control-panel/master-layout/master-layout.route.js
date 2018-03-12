module.exports = function (ngModule) {

    // Module html template import.
    var ngModuleHtmlTemplate = require('./master-layout.html');

    ngModule.config(function ($stateProvider, urlStates) {

        var urlStateControlPanel = urlStates.controlPanel;
        var urlStateControlPanelMasterLayout = urlStateControlPanel.masterLayout;
        var urlStateAuthorizedLayout = urlStates.authorizedLayout;

        $stateProvider.state(urlStateControlPanelMasterLayout.name, {
            url: urlStateControlPanelMasterLayout.url,
            controller: 'controlPanelMasterLayoutController',
            parent: urlStateAuthorizedLayout.name,
            template: ngModuleHtmlTemplate
        });
    });
};