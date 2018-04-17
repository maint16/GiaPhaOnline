/*
* Module exports.
* */
module.exports = function (ngModule) {
    ngModule.controller('userManagementController', function ($scope, toastr, $ngConfirm, $translate,
                                                              $timeout, $state, $compile, $interpolate,
                                                              appSettingConstant, urlStates, userRoleConstant, userStatusConstant,
                                                              DTOptionsBuilder, DTColumnBuilder,
                                                              profile,
                                                              moment, commonService, userService) {

        //#region Properties

        // Constant reflection.
        $scope.urlStates = urlStates;

        // Services reflection.
        $scope.userService = userService;

        // Data-table options.
        $scope.dtOptions = {
            userManagement: DTOptionsBuilder.newOptions()
                .withBootstrap()
                .withDataProp('data')
                .withDisplayLength(appSettingConstant.pagination.default)
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
                    var iPage = commonService.getDataTableStartIndex(startIndex, appSettingConstant.pagination.default);

                    if (!iPage)
                        iPage = 1;

                    // Build post searching condition.
                    var getUsersCondition = {
                        pagination: {
                            page: (!iPage || iPage < 1) ? 1 : iPage,
                            records: appSettingConstant.pagination.default
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
                        .then(function (getUsersResponse) {

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
                        .catch(function (getUsersError) {
                            fnCallback(items);
                        });
                })
        };

        // Data-table columns
        $scope.dtColumns = {
            userManagement: [
                // Email
                DTColumnBuilder.newColumn('email').withTitle($translate('Email')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        var szProfilePage = userService.getProfilePage(item.id);
                        return '<a ui-sref="' + szProfilePage + '">' + item.email + '</a>'
                    }
                ),
                // Nickname
                DTColumnBuilder.newColumn('nickname').withTitle($translate('Nickname')),
                // Status
                DTColumnBuilder.newColumn('status').withTitle($translate('Status')).notSortable()
                    .renderWith(function (data, type, item, meta) {
                        switch (item.status) {
                            case userStatusConstant.disabled:
                                return '<b class="text-bold text-danger">{{"Disabled" | translate}}</b>';
                            case userRoleConstant.pending:
                                return '<b class="text-bold text-gray">{{"Pending" | translate}}</b>';
                            default:
                                return '<b class="text-bold text-success">{{"Available" | translate}}</b>'
                        }
                    }),
                // Role
                DTColumnBuilder.newColumn(null).withTitle($translate('Role')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.role) {
                            case userRoleConstant.user:
                                return '<span>{{"User" | translate}}</span>';
                            default:
                                return '<b class="glyphicon glyphicon-fire"></b> <span>{{"Administrator" | translate}}</span>'
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
                        szUi += '<li ng-click="fnViewProfile(' + item.id + ')"><a href="javscript:void(0);"><span class="fa fa-eye"></span> {{"View" | translate}} </a></li>';

                        // Data construction.
                        var info = {
                            userId: item.id
                        };

                        // Viewer is the profile owner.
                        switch (item.status) {
                            case userStatusConstant.pending:
                                info['status'] = userStatusConstant.active;
                                szUi += $interpolate('<li ng-click="fnChangeUserStatus({{userId}}, {{status}})"><a href="javscript:void(0);"><span class="glyphicon glyphicon-gift"></span> <b class="text-info">{{"Activate" | translate}}</b> </a></li>')(info);
                                break;

                            case userStatusConstant.active:
                                info['status'] = userStatusConstant.disabled;
                                szUi += $interpolate('<li ng-click="fnChangeUserStatus({{userId}}, {{status}})"><a href="javscript:void(0);"><span class="fa fa-trash"></span> <b class="text-danger">{{"Disable" | translate}}</b> </a></li>')(info);
                                break;

                            case userStatusConstant.disabled:
                                info['status'] = userStatusConstant.active;
                                szUi += $interpolate('<li ng-click="fnChangeUserStatus({{userId}}, {{status}})"><a href="javscript:void(0);"><span class="fa fa-refresh"></span> <b class="text-success">{{"Restore" | translate}}</b> </a></li>')(info);
                                break;
                        }

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

        //#region Methods

        /*
        * Callback which is fired when view profile button is clicked.
        * */
        $scope.fnViewProfile = function (id) {
            $state.go(urlStates.user.profile.name, {profileId: id});
        };

        /*
        * Callback which is fired when change user status button is clicked.
        * */
        $scope.fnChangeUserStatus = function (id, status) {

            $ngConfirm({
                title: ' ',
                content: '<b class="text-bold text-warning">{{"Are you sure to change this account status ?" | translate}}</b>',
                theme: 'bootstrap',
                scope: $scope,
                buttons: {
                    ok: {
                        text: $translate.instant('OK'),
                        btnClass: 'btn btn-primary btn-flat',
                        action: function (scope, button) {
                            return userService.editUserStatus(id, status)
                                .then(function () {
                                    // Get translated message.
                                    var szMessage = $translate.instant('User status has been changed successfully');
                                    toastr.success(szMessage);

                                    // Reload user management table.
                                    $scope.dtInstances.userManagement.dataTable._fnDraw();

                                    return true;
                                })
                                .catch(function () {
                                    return false;
                                });
                        }
                    },
                    cancel: {
                        text: $translate.instant('Cancel'),
                        btnClass: 'btn btn-default btn-flat',
                        action: function (scope, button) {
                            return true;
                        }
                    }
                }
            });
        }

        //#endregion
    });
};