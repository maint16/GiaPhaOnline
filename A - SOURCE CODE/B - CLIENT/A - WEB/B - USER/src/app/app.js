'use strict';

// Css imports.
require('../../node_modules/bootstrap/dist/css/bootstrap.css');

// AdminLTE
require('../../node_modules/admin-lte/dist/css/AdminLTE.css');
require('../../node_modules/admin-lte/dist/css/skins/skin-green-light.css');

require('../../node_modules/angular-toastr/dist/angular-toastr.css');

// Import angularjs data-table.
require('../../node_modules/datatables.net-responsive-dt/css/responsive.dataTables.css');

// Font awesome.
require('../../node_modules/font-awesome/css/font-awesome.css');
require('../../node_modules/angular-block-ui/dist/angular-block-ui.css');
require('../../node_modules/datatables.net-bs/css/dataTables.bootstrap.css');
require('../../node_modules/angular-confirm1/css/angular-confirm.css');
require('../../node_modules/ui-cropper/compile/unminified/ui-cropper.css');
require('../../node_modules/ng-multi-selector/ng-multi-selector.css');
require('../app/loader.css');
require('../app/app.css');

// Import jquery lib.
require('jquery');
require('bluebird');
require('bootstrap');
require('admin-lte');
require('datatables.net/js/jquery.dataTables');
require('datatables.net-responsive');
require('moment');
require('pusher-js');
require('@aspnet/signalr/dist/cjs');

require('rxjs/bundles/rxjs.umd');
var firebase = require('firebase/app');
require('firebase/messaging');

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
require('angular-sanitize');
require('angular-confirm1');
require('ng-multi-selector');
require('angular-file-upload');
require('ui-cropper');
require('angular-messages');

// Module declaration.
var ngModule = angular.module('ngApp', ['ui.router', 'blockUI', 'toastr',
    'ui.bootstrap', 'ngMultiSelector',
    'pascalprecht.translate',
    'datatables', 'datatables.bootstrap', 'angularMoment', 'ngSanitize', 'ngMessages',
    'cp.ngConfirm', 'angularFileUpload', 'uiCropper']);

ngModule.config(function ($urlRouterProvider, $translateProvider, $httpProvider,
                          blockUIConfig,
                          urlStates) {


    // API interceptor
    $httpProvider.interceptors.push('apiInterceptor');

    // Url router config.
    $urlRouterProvider.otherwise(urlStates.dashboard.url);

    // Translation config.
    $translateProvider.useStaticFilesLoader({
        prefix: './assets/dictionary/',
        suffix: '.json'
    });

    // Use sanitize.
    $translateProvider.useSanitizeValueStrategy('sanitize');

    // en-US is default selection.
    $translateProvider.use('en-US');

    // BlockUI configuration.
    blockUIConfig.autoInjectBodyBlock = false;
    // blockUIConfig.template = '<div block-ui-container="" class="block-ui-container ng-scope"><div class="block-ui-overlay"></div><div class="block-ui-message-container" aria-live="assertive" aria-atomic="true"><div class="loader"></div></div></div>';
    blockUIConfig.templateUrl = 'main-block-ui.html';
});

// Import angular-dataTable configs.
require('./configs/angular-dataTable.config')(ngModule);

// Import pager controller configs.
require('./configs/angular-paginator.config')(ngModule);

/*
* Application controller.
* */
ngModule.controller('appController', function ($transitions, $timeout,
                                               urlStates,
                                               uiService, pushNotificationService,
                                               $scope) {

    //#region Properties

    // For two-way model binding.
    $scope.model = {
        layoutClass: ''
    };

    // Check whether fcm service has been initialized before.
    $scope.bIsFcmInitialized = false;

    //#endregion

    //#region Methods

    //#endregion

    //#region Watchers & events

    /*
    * Called when transition from state to state is successful.
    * */
    $transitions.onSuccess({}, function ($transition) {

        $timeout(function () {

            // Reload window size.
            uiService.reloadWindowSize();

            if (!$scope.bIsFcmInitialized){
                // Initialize firebase.
                firebase.initializeApp({
                    'messagingSenderId': '420319602777'
                });

                // Request for push notification service.
                var messaging = firebase.messaging();
                messaging.usePublicVapidKey("BKa5DHBtGv4JchD7XuP571kHQKM-7T-5Bdj5KM3flRjVFDVtTDtX6CEe_WEeHuwmoV0O1DaQ7KClP6kqG618--A");
                messaging.requestPermission()
                    .then(function () {
                        return messaging.getToken()
                            .then(function (fcmToken) {
                                return fcmToken;
                            })
                            .catch(function (error) {
                                console.log('An error occurred while retrieving token. ', err);
                                throw 'An error occurred while retrieving token.';
                            });
                    })
                    .then(function(fcmToken){

                        // Initialize add device condition.
                        var addDeviceCondition = {
                            deviceId: fcmToken
                        };

                        // Mark fcm service as has been initialized.
                        $scope.bIsFcmInitialized = true;

                        // Call api service to add device.
                        return pushNotificationService.addDevice(addDeviceCondition);
                    })
                    .catch(function (error) {
                        console.log('Unable to get permission to notify.');
                    });
            }


        }, 250);

        // Find destination of transaction.
        var destination = $transition.$to();

        if (destination.includes[urlStates.authorizedLayout.name]) {
            $scope.model.layoutClass = 'hold-transition skin-green-light layout-top-nav';
            return;
        }

        var urlStateUser = urlStates.user;
        if (destination.includes[urlStateUser.login.name] || destination.includes[urlStateUser.googleLogin.name]) {
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

