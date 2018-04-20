/*
* Module exports.
* */
module.exports = function (ngModule) {
    ngModule.controller('userManagementController', function ($scope, toastr, $ngConfirm, $translate,
                                                              $timeout, $state, $compile, $interpolate, $uibModal,
                                                              appSettingConstant, urlStates,
                                                              userRoleConstant, userStatusConstant,
                                                              DTOptionsBuilder, DTColumnBuilder,
                                                              profile,
                                                              moment, commonService, userService) {

        //#region Properties

        // Constant reflection.
        $scope.urlStates = urlStates;

        // Services reflection.
        $scope.userService = userService;

        // View temporary cache.
        $scope.cache = {
            users: {}
        };

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

                    // Block UI.
                    commonService.blockAppUI();

                    userService.getUsers(getUsersCondition)
                        .then(function (getUsersResponse) {

                            // Invalid user result.
                            var getUserResult = getUsersResponse.data;
                            if (!getUserResult) {
                                fnCallback(items);
                                return;
                            }

                            // Build items list.
                            var users = getUserResult.records;
                            var cachedUsers = {};
                            angular.forEach(users, function (user) {
                                cachedUsers[user.id] = user;
                            });

                            // Add users to cache.
                            $scope.cache.users = cachedUsers;

                            items.data = users;
                            items.draw = draw;
                            items.recordsTotal = getUserResult.total;
                            items.recordsFiltered = getUserResult.total;
                            fnCallback(items);
                        })
                        .catch(function (getUsersError) {
                            fnCallback(items);
                        })
                        .finally(function () {
                            commonService.unblockAppUI();
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
                DTColumnBuilder.newColumn('nickname').withTitle($translate('Nickname')).notSortable(),
                // Status
                DTColumnBuilder.newColumn('status').withTitle($translate('Status')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.status) {
                            case userStatusConstant.disabled:
                                return '<b class="text-danger">{{"Disabled" | translate}}</b>';
                            case userStatusConstant.pending:
                                return '<b class="text-gray">{{"Pending" | translate}}</b>';
                            default:
                                return '<b class="text-success">{{"Available" | translate}}</b>';
                        }
                    }
                ),
                // Role
                DTColumnBuilder.newColumn(null).withTitle($translate('Role')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.role) {
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
                        szUi += '{{"Action" | translate}} ';
                        szUi += '<span class="caret"></span>';
                        szUi += '</button>';
                        szUi += '<ul class="dropdown-menu" aria-labelledby="dropdownMenu1">';
                        szUi += '<li><a href="javascript:void(0);"><span class="fa fa-eye"></span> {{"View" | translate}} </a></li>';

                        // Viewer is the profile owner.
                        if (profile.id !== item.id) {
                            switch (item.status) {
                                case userStatusConstant.pending:
                                    szUi += $interpolate('<li ng-click="editUserStatus({{item.id}})"><a href="javascript:void(0);"><span class="fa fa-check"></span> <b class="text-green">{{"Activate" | translate}}</b> </a></li>')({item: item});
                                    break;

                                case userStatusConstant.disabled:
                                    szUi += $interpolate('<li ng-click="editUserStatus({{item.id}})"><a href="javascript:void(0);"><span class="fa fa-refresh"></span> <b class="text-success">{{"Restore" | translate}}</b> </a></li>')({item: item});
                                    break;

                                default:
                                    szUi += $interpolate('<li ng-click="editUserStatus({{item.id}})"><a href="javascript:void(0);"><span class="fa fa-trash"></span> <b class="text-danger"></b> {{"Delete" | translate}}</a></li>')({item: item});
                                    break;
                            }
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
        * Callback which is raised when change user status button is clicked.
        * */
        $scope.editUserStatus = function (id) {

            // User is not in cache.
            var user = $scope.cache.users[id];
            if (!user)
                return;

            var editUserStatusModal = $uibModal.open({
                templateUrl: 'edit-user-status.tmpl.html',
                size: 'md',
                controller: 'editUserStatusController',
                resolve: {
                    user: function () {
                        return user;
                    }
                }
            });

            editUserStatusModal.closed.then(function () {
                $scope.dtInstances.userManagement.dataTable._fnDraw();
            });
        };

        //#endregion
    })
        .controller('editUserStatusController', function (
            user,
            userStatusConstant,
            commonService, userService,
            $uibModalInstance,
            $scope) {

            //#region Properties

            // Resolver reflection.
            $scope.user = user;

            // Constant reflection.
            $scope.userStatusConstant = userStatusConstant;

            // Model for information binding.
            $scope.model = {
                reason: null
            };

            //#endregion

            //#region Methods

            /*
            * Callback which is fired when user status is submitted.
            * */
            $scope.editUserStatus = function () {

                // Form is not valid.
                var editUserStatusForm = $scope.editUserStatusForm;
                if (!editUserStatusForm || !editUserStatusForm.$valid)
                    return;

                var designatedStatus = null;
                switch (user.status) {
                    case userStatusConstant.disabled:
                    case userStatusConstant.pending:
                        designatedStatus = userStatusConstant.active;
                        break;

                    default:
                        designatedStatus = userStatusConstant.disabled;
                }

                // Block application UI.
                commonService.blockAppUI();

                return userService.editUserStatus(user.id, designatedStatus, $scope.model.reason)
                    .then(function () {
                        // Close modal instance.
                        $uibModalInstance.close();
                        return true;
                    })
                    .finally(function () {
                        // Unblock UI.
                        commonService.unblockAppUI();
                    })

            };

            //#endregion
        });
};