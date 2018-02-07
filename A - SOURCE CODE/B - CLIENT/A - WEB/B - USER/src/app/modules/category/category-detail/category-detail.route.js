module.exports = function (ngModule) {

    // Module html template import.
    var ngModuleHtmlTemplate = require('./category-detail.html');

    ngModule.config(function ($stateProvider, urlStates) {

        var urlStateCategoryDetail = urlStates.category.postListing;
        var urlStateAuthorizedLayout = urlStates.authorizedLayout;

        $stateProvider.state(urlStateCategoryDetail.name, {
            url: urlStateCategoryDetail.url,
            controller: 'categoryDetailController',
            parent: urlStateAuthorizedLayout.name,
            template: ngModuleHtmlTemplate
        });
    });
};