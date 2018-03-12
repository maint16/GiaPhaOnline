module.exports = function (ngModule) {

    // Module html template import.
    var ngModuleHtmlTemplate = require('./category-management.html');

    ngModule.config(function ($stateProvider, urlStates) {

        var urlStateControlPanel = urlStates.controlPanel;
        var urlStateControlPanelMasterLayout = urlStateControlPanel.masterLayout;
        var urlCategoryManagement = urlStateControlPanel.categoryManagement;

        $stateProvider.state(urlCategoryManagement.name, {
            url: urlCategoryManagement.url,
            controller: 'categoryManagementController',
            parent: urlStateControlPanelMasterLayout.name,
            template: ngModuleHtmlTemplate
        });
    });
};