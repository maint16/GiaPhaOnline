/*
* Module exports.
* */
module.exports = function (ngModule) {
    ngModule.controller('userManagementController', function ($scope, toastr, $ngConfirm, $translate,
                                                              $timeout, $state, $compile,
                                                              appSettings, urlStates, userRoleConstant,
                                                              DTOptionsBuilder, DTColumnBuilder,
                                                              profile,
                                                              moment, commonService, userService) {

        //#region Properties

        // Data-table options.
        $scope.dtOptions = {
            userManagement: DTOptionsBuilder.newOptions()
                .withBootstrap()
                .withDataProp('data')
                .withDisplayLength(appSettings.pagination.default)
                .withOption('responsive', true)
                .withDOM('<"top"i>rt<"dt-center-pagination"flp><"clear">')
                .withOption('fnRowCallback',
                    function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                        $compile(nRow)($scope);
                    })
                .withFnServerData(function (sSource, aoData, fnCallback, oSettings) {
                    // Drawing counter.
                    var draw = aoData[3].value;

                    // Start index calculation.
                    var startIndex = aoData[3].value;
                    var iPage = commonService.getDataTableStartIndex(startIndex, appSettings.pagination.default);

                    if (!iPage)
                        iPage = 1;

                    // Build post searching condition.
                    var getUsersCondition = {
                        pagination: {
                            page: (!iPage || iPage < 1) ? 1 : iPage,
                            records: appSettings.pagination.default
                        }
                    };

                    // Item which is for drawing table.
                    var items = {
                        draw: 0,
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        data: []
                    };

                    userService.getUsers(getUsersCondition)
                        .then(function(getUsersResponse){

                            // Invalid user result.
                            var getUserResult = getUsersResponse.data;
                            if (!getUserResult) {
                                fnCallback(items);
                                return;
                            }

                            // Build items list.
                            items.data = getUserResult.records;
                            items.draw = draw;
                            items.recordsTotal = getUserResult.total;
                            items.recordsFiltered = getUserResult.total;
                            fnCallback(items);
                        })
                        .catch(function(getUsersError){
                            fnCallback(items);
                        });
                })
        };

        // Data-table columns
        $scope.dtColumns = {
            userManagement: [
                // Email
                DTColumnBuilder.newColumn('email').withTitle($translate('Email')).notSortable(),
                // Nickname
                DTColumnBuilder.newColumn('nickname').withTitle($translate('Nickname')).notSortable(),
                // Status
                DTColumnBuilder.newColumn('status').withTitle($translate('Status')).notSortable(),
                // Role
                DTColumnBuilder.newColumn(null).withTitle($translate('Role')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.role){
                            case userRoleConstant.user:
                                return '<span>{{"User" | translate}}</span>';
                            default:
                                return '<span>{{"Administrator" | translate}}</span>'
                        }
                    }
                ),
                // Created time
                DTColumnBuilder.newColumn(null).withTitle($translate('Joined time')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return moment(item.joinedTime).format('LLL');
                    }
                ),
                // Last modified time
                DTColumnBuilder.newColumn(null).withTitle($translate('Last modified time')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        if (item.lastModifiedTime == null)
                            return '';
                        return moment(item.lastModifiedTime).format('LLL');
                    }
                ),
                // Action
                DTColumnBuilder.newColumn(null).notSortable().renderWith(
                    function (data, type, item, meta) {
                        var szUi = '';
                        szUi += '<div class="dropdown">';
                        szUi += '<button class="btn btn-default btn-flat dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">';
                        szUi += $translate.instant('Action') + ' ';
                        szUi += '<span class="caret"></span>';
                        szUi += '</button>';
                        szUi += '<ul class="dropdown-menu" aria-labelledby="dropdownMenu1">';
                        szUi += '<li><a href="javscript:void(0);"><span class="fa fa-eye"></span> ' + $translate.instant('View') + ' </a></li>';

                        // Viewer is the profile owner.
                        szUi += '<li><a href="javscript:void(0);"><span class="fa fa-trash"></span> ' + $translate.instant('Delete') + ' </a></li>';

                        szUi += '</ul>';
                        szUi += '</div>';
                        return szUi;
                    }
                )
            ]
        };

        // Data-table instance.
        $scope.dtInstances = {
            userManagement: {}
        };

        //#endregion
    });
};