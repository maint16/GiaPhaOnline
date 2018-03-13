/*
* Module exports.
* */
module.exports = function (ngModule) {
    ngModule.controller('categoryManagementController', function ($scope, toastr, $ngConfirm, $translate,
                                                                  $timeout, $state,
                                                                  appSettings, urlStates, itemStatusConstant,
                                                                  profile,
                                                                  DTOptionsBuilder, DTColumnBuilder,
                                                                  moment, $compile, $interpolate,
                                                                  commonService, categoryService, userService) {

        //#region Properties

        // Buffer for temporary caching.
        $scope.buffer = {
            categories: {},
            users: {}
        };

        // Data-table instance.
        $scope.dtOptions = {
            categoryManagement: DTOptionsBuilder.newOptions()
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
                    var getCategoriesCondition = {
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

                    // Clear buffer.
                    $scope.buffer.categories = {};

                    // Get a list of categories.
                    categoryService.getCategories(getCategoriesCondition)
                        .then(function(getCategoriesResponse){ // Get list of categories.

                            // Invalid user result.
                            var getCategoriesResult = getCategoriesResponse.data;
                            if (!getCategoriesResult) {
                                fnCallback(items);
                                return;
                            }

                            return getCategoriesResult;
                        })
                        .then(function(getCategoriesResult){ // Get list of category creators.

                            // Get the list of categories.
                            var categories = getCategoriesResult.records;

                            // Categories list is empty.
                            if (!categories || categories.length < 1)
                                return getCategoriesResult;

                            // List of users that aren't in buffer.
                            var userIds = [];

                            // Get through every categories and retrieve the users who aren't in buffer.
                            angular.forEach(categories, function(category, iterator){

                                // Update category into buffer.
                                $scope.buffer.categories[category.id] = category;

                                // User is already in buffer.
                                if ($scope.buffer.users[category.creatorId])
                                    return;

                                userIds.push(category.creatorId);
                            });

                            // Users list has been found.
                            if (userIds && userIds.length > 0){
                                // Call api end-point to get a list of users.
                                var loadUsersCondition = {
                                    ids: userIds
                                };

                                return userService.loadUsers(loadUsersCondition)
                                    .then(function(loadUsersResponse){
                                        var loadUsersResult = loadUsersResponse.data;
                                        if (!loadUsersResult)
                                            return getCategoriesResult;

                                        var users = loadUsersResult.records;
                                        var bufferedUsers = {};
                                        angular.forEach(users, function(user, iterator){
                                            bufferedUsers[user.id] = user;
                                        });

                                        $scope.buffer.users = bufferedUsers;
                                        return getCategoriesResult;
                                    });
                            }

                            return getCategoriesResult;
                        })
                        .then(function(getCategoriesResult){
                            // Build items list.
                            items.data = getCategoriesResult.records;
                            items.draw = draw;
                            items.recordsTotal = getCategoriesResult.total;
                            items.recordsFiltered = getCategoriesResult.total;
                            fnCallback(items);
                        })
                        .catch(function(getUsersError){
                            fnCallback(items);
                        });
                })
        };

        // Data-table columns
        $scope.dtColumns = {
            categoryManagement: [
                // Name
                DTColumnBuilder.newColumn(null).withTitle($translate('Name')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.status){
                            case itemStatusConstant.deleted:
                                return '<span class="text-danger">' + item.name + '</span>';
                            default:
                                return '<span class="text-success">' + item.name + '</span>';
                        }
                    }
                ),
                // Status
                DTColumnBuilder.newColumn('status').withTitle($translate('Status')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.status){
                            case itemStatusConstant.deleted:
                                return '<span class="text-danger">{{"Deleted" | translate}}</span>';
                            default:
                                return '<span class="text-success">{{"Available" | translate}}</span>'
                        }
                    }
                ),
                // Creator
                DTColumnBuilder.newColumn(null).withTitle($translate('Creator')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return '<a ui-sref="' + userService.getProfilePage(item.creatorId) +'">' + $scope.buffer.users[item.creatorId].nickname + '</a>';
                    }
                ),
                // Created time
                DTColumnBuilder.newColumn(null).withTitle($translate('Created time')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return moment(item.createdTime).format('LLL');
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

                        // View button
                        szUi += '<li><a href="javscript:void(0);" ui-sref="' + $scope.getCategoryPage(item.id) + '"><span class="fa fa-eye"></span> ' + $translate.instant('View') + ' </a></li>';

                        if (item.status !== itemStatusConstant.deleted) {
                            szUi += '<li ng-click="fnDeleteCategory(' + item.id + ')"><a href="javscript:void(0);"><span class="fa fa-trash"></span> ' + $translate.instant('Delete') + ' </a></li>';
                        } else {
                            szUi += '<li><a href="javscript:void(0);"><span class="fa fa-refresh"></span> ' + $translate.instant('Restore') + ' </a></li>';
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
            categoryManagement: {}
        };


        //#endregion

        //#region Methods

        /*
        * Get category page by using id.
        * */
        $scope.getCategoryPage = function(categoryId){
            return urlStates.category.postListing.name + '({categoryId:' + categoryId + '})';
        };

        /*
        * Function is called when category delete button is clicked.
        * */
        $scope.fnDeleteCategory = function(id){

            // Category is not in buffer.
            if (!$scope.buffer.categories[id])
                return;

            // Find the category from buffer.
            var category = $scope.buffer.categories[id];

            // Category has been deleted before.
            if (category.status !== itemStatusConstant.available)
                return;

            $ngConfirm({
                title: ' ',
                content: '<strong class="text-danger">{{"Are you sure to delete this category ?" | translate}}</strong>',
                scope: $scope,
                buttons: {
                    ok: {
                        text: $translate.instant('Ok'),
                        btnClass: 'btn btn-flat btn-danger',
                        action: function(scope, button){
                            scope.name = 'Booo!!';
                            return false; // prevent close;
                        }
                    },
                    cancel: {
                        text: $translate.instant('Cancel'),
                        btnClass: 'btn btn-flat btn-default',
                        action: function(scope, button){
                        }
                    }
                }
            });

        }

        //#endregion


    });
};