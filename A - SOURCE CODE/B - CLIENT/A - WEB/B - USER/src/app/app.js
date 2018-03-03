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
require('../../node_modules/datatables.net-bs/css/dataTables.bootstrap.css');
require('../../node_modules/summernote/dist/summernote.css');
require('../../node_modules/angular-confirm1/css/angular-confirm.css');
require('../../node_modules/ui-cropper/compile/unminified/ui-cropper.css');
require('../../src/app/app.css');

// Import jquery lib.
require('jquery');
require('bluebird');
require('bootstrap');
require('admin-lte');
require('datatables.net/js/jquery.dataTables');
require('moment');
require('codemirror');
require('summernote');
require('rxjs/bundles/Rx');

// Angular plugins declaration.
var angular = require('angular');
require('@uirouter/angularjs');
require('angular-block-ui');
require('angular-toastr');
require('angular-translate');
require('angular-translate-loader-static-files');
require('angular-datatables');
require('angular-moment');
require('angular-ui-bootstrap');
require('angular-summernote/dist/angular-summernote');
require('angular-sanitize');
require('angular-confirm1');
require('ng-multi-selector');
require('angular-file-upload');
require('ui-cropper');

// Module declaration.
var ngModule = angular.module('ngApp', ['ui.router', 'blockUI', 'toastr',
    'ui.bootstrap', 'ngMultiSelector',
    'pascalprecht.translate',
    'datatables', 'datatables.bootstrap', 'angularMoment', 'summernote', 'ngSanitize',
    'cp.ngConfirm', 'angularFileUpload', 'uiCropper']);

ngModule.config(function($urlRouterProvider, $translateProvider, $httpProvider, urlStates){

    // API interceptor
    $httpProvider.interceptors.push('apiInterceptor');

    // Url router config.
    $urlRouterProvider.otherwise(urlStates.dashboard.url);

    // Translation config.
    $translateProvider.useStaticFilesLoader({
        prefix: './assets/dictionary/',
        suffix: '.json'
    });

    // en-US is default selection.
    $translateProvider.use('en-US');

});

// Import angular-dataTable configs.
require('./configs/angular-dataTable.config')(ngModule);

// Import pager controller configs.
require('./configs/angular-paginator.config')(ngModule);

/*
* Application controller.
* */
ngModule.controller('appController', function($transitions, $timeout,
                                              urlStates,
                                              uiService,
                                              $scope){

    //#region Properties

    // For two-way model binding.
    $scope.model = {
        layoutClass: ''
    };

    //#endregion

    //#region Methods

    //#endregion

    //#region Watchers & events

    /*
    * Called when transition from state to state is successful.
    * */
    $transitions.onSuccess({}, function($transition){

        $timeout(function() {
            uiService.reloadWindowSize();
        }, 250);

        // Find destination of transaction.
        var destination = $transition.$to();

        if (destination.includes[urlStates.authorizedLayout.name]){
            $scope.model.layoutClass = 'hold-transition skin-green-light layout-top-nav';
            return;
        }

        var urlStateUser = urlStates.user;
        if (destination.includes[urlStateUser.login.name] || destination.includes[urlStateUser.googleLogin.name]){
            $scope.model.layoutClass = 'hold-transition login-page';
            return;
        }

        $scope.model.layoutClass = 'hold-transition';
    });

    //#endregion
});

// Constants import.
require('./constants/index')(ngModule);

// Factories import.
require('./factories/index')(ngModule);

// Services import.
require('./services/index')(ngModule);

// Directive requirements.
require('./directives/index')(ngModule);

// Module requirements.
require('./modules/index')(ngModule);