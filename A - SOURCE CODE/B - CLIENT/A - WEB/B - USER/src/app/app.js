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
require('./styles/app.scss');

// Import jquery lib.
require('jquery');
require('bluebird');
require('bootstrap');
require('admin-lte');
require('moment');
require('pusher-js');

require('rxjs/bundles/rxjs.umd');
const firebase = require('firebase');
require('firebase/messaging');

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
require('angular-file-upload');
require('ui-cropper');
require('angular-messages');

$.ajax({
    url: '/assets/app-settings.json',
    contentType: 'application/json',
    method: 'GET',
    cache: false,
    crossDomain: false,
    success: (loadAppSettings) => {

        // Bind the app setting to window object.
        window.app = loadAppSettings;

        // Module declaration.
        let ngModule = angular.module('ngApp', ['ui.router', 'blockUI', 'toastr',
            'ui.bootstrap', 'ngMultiSelector', 'ngMessages', 'oc.lazyLoad',
            'pascalprecht.translate', 'angularMoment', 'ngSanitize',
            'cp.ngConfirm', 'angularFileUpload', 'uiCropper']);

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

        require('./app.controller')(ngModule);
        require('./configs')(ngModule);

        // Constants import.
        require('./constants')(ngModule);

        // Factories import.
        require('./factories')(ngModule);

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




