module.exports = function (ngModule) {

    // Module html template import.
    var ngModuleHtmlTemplate = require('./main.html');

    ngModule.config(function ($stateProvider, urlStates) {

        var urlStateDashboard = urlStates.dashboard;
        var urlStateAuthorizedLayout = urlStates.authorizedLayout;

        $stateProvider.state(urlStateDashboard.name, {
            url: urlStateDashboard.url,
            controller: 'mainDashboardController',
            parent: urlStateAuthorizedLayout.name,
            template: ngModuleHtmlTemplate,
            resolve: {

                // List of default categories.
                initialGetCategory: function (categoryService, appSettings) {

                    var condition = {
                        pagination: {
                            page: 1,
                            records: appSettings.pagination.default
                        }
                    };

                    // Get list of categories.
                    return categoryService.getCategories(condition)
                        .then(function(getCategoriesResponse){

                            // Get api result.
                            return getCategoriesResponse.data;
                        });
                }
            }
        });
    });
};