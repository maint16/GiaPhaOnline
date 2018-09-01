'use strict';

// Css imports.
require('../../node_modules/bootstrap/dist/css/bootstrap.css');

// AdminLTE
require('../../node_modules/admin-lte/dist/css/AdminLTE.css');
require('../../node_modules/admin-lte/dist/css/skins/skin-green-light.css');

require('../../node_modules/angular-toastr/dist/angular-toastr.css');

// Font awesome.
require('../../node_modules/font-awesome/css/font-awesome.css');
require('../../node_modules/angular-block-ui/dist/angular-block-ui.css');
require('../../node_modules/angular-confirm1/css/angular-confirm.css');
require('../../node_modules/ui-cropper/compile/unminified/ui-cropper.css');
require('../../node_modules/ng-multi-selector/ng-multi-selector.css');
require('../../node_modules/tinymce/skins/lightgray/skin.min.css');

require('./styles/app.scss');

// Import jquery lib.
require('jquery');
require('bluebird');
require('bootstrap');
require('moment');
require('pusher-js');
require('tinymce');
require('tinymce/themes/modern');

require('rxjs/bundles/rxjs.umd');
const firebase = require('firebase');
require('firebase/messaging');


$.ajax({
    url: '/assets/app-settings.json',
    contentType: 'application/json',
    method: 'GET',
    cache: false,
    crossDomain: false,
    success: (loadAppOptionResponse) => {

        // Angular plugins declaration.
        const angular = require('angular');
        require('@uirouter/angularjs');
        require('oclazyload');
        require('angular-block-ui');
        require('angular-toastr');
        require('angular-translate');
        require('angular-translate-loader-static-files');
        require('angular-moment');
        require('angular-ui-bootstrap');
        require('angular-sanitize');
        require('angular-confirm1');
        require('ng-multi-selector');
        require('angular-messages');
        require('angular-ui-tinymce');
        require('angular-local-storage');
        require('angular-recaptcha');

        // Bind the app setting to window object.
        window['_ngAppOptions'] = loadAppOptionResponse;

        // Module declaration.
        let ngModule = angular.module('ngApp', ['ui.router', 'blockUI', 'toastr',
            'ui.bootstrap', 'ngMultiSelector', 'ngMessages', 'oc.lazyLoad', 'vcRecaptcha',
            'pascalprecht.translate', 'angularMoment', 'ngSanitize', 'ui.tinymce',
            'cp.ngConfirm', 'LocalStorageModule']);

        // Import url state constant
        const UrlStateConstant = require('./constants/url-state.constant.ts').UrlStateConstant;

        ngModule
            .config(($urlRouterProvider, $translateProvider, $httpProvider,
                     blockUIConfig) => {

                // API interceptor
                $httpProvider.interceptors.push('apiInterceptor');

                // Url router config.
                $urlRouterProvider.otherwise(UrlStateConstant.dashboardModuleUrl);

                // Translation config.
                $translateProvider.useStaticFilesLoader({
                    prefix: './assets/dictionary/',
                    suffix: '.json'
                });

                // Use sanitize.
                $translateProvider.useSanitizeValueStrategy('sanitize');

                // en-US is default selection.
                $translateProvider.use('en-US');
            });

        // Import configurations.
        require('./configs')(ngModule);

        // Constants import.
        require('./constants')(ngModule);

        // Factories import.
        require('./factories')(ngModule);

        // Filters import.
        require('./filters')(ngModule);

        // Services import.
        require('./services')(ngModule);

        // Directive requirements.
        require('./directives')(ngModule);

        // Module requirements.
        require('./modules')(ngModule);

        // Manually bootstrap the application.
        angular.bootstrap(document, ['ngApp']);
    }
});




