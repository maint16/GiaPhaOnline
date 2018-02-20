module.exports = function (ngModule) {

    //#region Module configs.

    // Load module html template.
    var ngModuleHtmlTemplate = require('./login.html');

    /*
    * Module configuration.
    * */
    ngModule.config(function ($stateProvider, urlStates) {

        // Get state parameter.
        var urlLoginState = urlStates.user.login;

        $stateProvider.state(urlLoginState.name, {
            url: urlLoginState.url,
            controller: 'loginController',
            template: ngModuleHtmlTemplate,
            parent: urlStates.unauthorizedLayout.name
        })
    });

    //#endregion
};