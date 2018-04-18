module.exports = function (ngModule) {

    // Import module template.
    var ngModuleHtmlTemplate = require('./user-picker.html');

    // Directive declaration.
    ngModule.directive('userPicker', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'E',
            scope: {
                ngIsHeaderAvailable: '=',
                ngSelectUser: '&',
                ngClickCancel: '&'
            },
            controller: function ($scope, $translate, $compile,
                                  userService, commonService,
                                  appSettingConstant,
                                  DTOptionsBuilder, DTColumnBuilder) {

                // Whether component is busy or not.
                $scope.bIsBusy = false;

                // Buffer data.
                $scope.buffer = {
                    users : {}
                };

                //#region Properties

                // Options of data-tables.
                $scope.dtOptions = {
                    userPicker: DTOptionsBuilder.newOptions()
                        .withBootstrap()
                        .withDataProp('data')
                        .withDisplayLength(appSettingConstant.pagination.userSelector)
                        .withOption('processing', true)
                        .withOption('serverSide', true)
                        .withOption('fnRowCallback',
                            function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                                $compile(nRow)($scope);
                            })
                        .withFnServerData(function (sSource, aoData, fnCallback, oSettings) {

                            // Draw time (not sure what it does in angularjs datatable)
                            var draw = aoData[0].value;

                            // Start element index.
                            var startIndex = aoData[3].value;

                            // Page calculation.
                            var iPage = commonService.getDataTableStartIndex(startIndex, appSettingConstant.pagination.default);

                            if (!iPage)
                                iPage = 1;

                            // Search condition initialization.
                            var conditions = {
                                pagination: {
                                    page: iPage,
                                    records: appSettingConstant.pagination.userSelector
                                }
                            };

                            if ($scope.model && $scope.model.username && $scope.model.username.length > 0) {
                                conditions.username = $scope.model.username;
                            }

                            // Clear the result.
                            $scope.getUserResult = null;

                            // Call api to get data.
                            userService.getUsers(conditions).then(
                                function success(x) {

                                    // Release busy status.
                                    $scope.bIsBusy = false;

                                    var items = {
                                        draw: 0,
                                        recordsTotal: 0,
                                        recordsFiltered: 0,
                                        data: []
                                    };

                                    var result = x.data;
                                    if (!result) {
                                        fnCallback(items);
                                        return;
                                    }

                                    items.draw = draw;
                                    items.recordsTotal = result.total;
                                    items.recordsFiltered = result.total;
                                    items.data = result.records;
                                    $scope.getUserResult = result;
                                    fnCallback(items);
                                },
                                function error(x) {
                                    $scope.bIsBusy = false;
                                });
                        })
                };

                // Column settings of data-tables.
                $scope.dtColumns = {
                    userPicker: [
                        DTColumnBuilder.newColumn('email').withTitle($translate('Email')).notSortable(),
                        DTColumnBuilder.newColumn('nickname').withTitle($translate('Nick name')).notSortable(),
                        // DTColumnBuilder.newColumn('staffNo').withTitle($translate('staffNo')).notSortable(),
                        DTColumnBuilder.newColumn(null).withTitle('').notSortable().renderWith(function (data, type, full) {
                            $scope.buffer.users[full.id] = full;
                            return '<button type="button" class="btn btn-flat btn-primary" ng-click="selectUser(\'' + full.id + '\')"><span class="glyphicon glyphicon-ok"></span></button>'
                        })
                    ]
                };
                /*
                * Model which is for information binding.
                * */
                $scope.model = {
                    email: null,
                    nickname: null
                };

                /*
                * Get user result.
                * */
                $scope.getUserResult = {
                    records: null,
                    total: 0
                };

                //#endregion

                //#region Methods

                /*
                * Fired when directive start to be initialized.
                * */
                $scope.init = function () {

                };

                /*
                * Fired when user is selected.
                * */
                $scope.selectUser = function (id) {
                    $scope.ngSelectUser({user: $scope.buffer.users[id]});
                };

                /*
                * Event which is fired when cancel button is clicked.
                * */
                $scope.cancel = function () {
                    $scope.ngClickCancel();
                };
                //#endregion
            }
        }
    });
};