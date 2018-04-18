module.exports = function (ngModule) {

    /*
    * Initialize controller with injectors.
    * */
    ngModule.controller('profileController', function (profile, personalProfile,
                                                       DTOptionsBuilder, DTColumnBuilder,
                                                       postTypeConstant, postStatusConstant, commentStatusConstant, appSettingConstant, userRoleConstant,
                                                       $ngConfirm, $uibModal, $translate, $state, $compile, moment, $interpolate,
                                                       $timeout, toastr,
                                                       uiService, userService, authenticationService, commonService, postService,
                                                       commentService, commentReportService, postReportService,
                                                       FileUploader,
                                                       $scope) {

        //#region Properties

        // Resolver reflection.
        $scope.personalProfile = personalProfile;
        $scope.userRoleConstant = userRoleConstant;

        // Modal dialog instances.
        $scope.modals = {
            changePassword: null,
            changeProfileImage: null
        };

        // Model for data binding.
        $scope.model = {
            changePassword: {
                currentPassword: null,
                newPassword: null,
                confirmPassword: null
            },

            // Profile image.
            profile: {
                originalImage: null,
                croppedImage: null
            }
        };

        /*
        * Data-table configurations
        * */
        $scope.dtOptions = {

            // Posts list
            posts: DTOptionsBuilder.newOptions()
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
                    var getPostsCondition = {
                        ownerId: $scope.personalProfile.id,
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

                    postService.getPosts(getPostsCondition)
                        .then(
                            function success(getPostsResponse) {
                                var getPostsResult = getPostsResponse.data;

                                // Result is invalid.
                                if (!getPostsResult) {
                                    fnCallback(items);
                                    return;
                                }

                                // Build items list.
                                items.data = getPostsResult.records;
                                items.draw = draw;
                                items.recordsTotal = getPostsResult.total;
                                items.recordsFiltered = getPostsResult.total;
                                fnCallback(items);
                            },
                            function error() {
                                fnCallback(items);
                                return;
                            });
                }),

            // Post reports list.
            postReports: DTOptionsBuilder.newOptions()
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
                    var getPostReportsCondition = {
                        reporterId: $scope.personalProfile.id,
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

                    postReportService.getPostReports(getPostReportsCondition)
                        .then(
                            function success(getPostReportsCondition) {
                                var getPostReportResult = getPostReportsCondition.data;

                                // Result is invalid.
                                if (!getPostReportResult) {
                                    fnCallback(items);
                                    return;
                                }

                                // Build items list.
                                items.data = getPostReportResult.records;
                                items.draw = draw;
                                items.recordsTotal = getPostReportResult.total;
                                items.recordsFiltered = getPostReportResult.total;
                                fnCallback(items);
                            },
                            function error() {
                                fnCallback(items);
                                return;
                            });
                }),

            // Comments list
            comments: DTOptionsBuilder.newOptions()
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
                    var getCommentsCondition = {
                        ownerId: $scope.personalProfile.id,
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

                    commentService.getComments(getCommentsCondition)
                        .then(
                            function success(getCommentsResponse) {
                                var getCommentsResult = getCommentsResponse.data;

                                // Result is invalid.
                                if (!getCommentsResult) {
                                    fnCallback(items);
                                    return;
                                }

                                // Build items list.
                                items.data = getCommentsResult.records;
                                items.draw = draw;
                                items.recordsTotal = getCommentsResult.total;
                                items.recordsFiltered = getCommentsResult.total;
                                fnCallback(items);
                            },
                            function error() {
                                fnCallback(items);
                                return;
                            });
                }),

            // Comment reports list.
            commentReports: DTOptionsBuilder.newOptions()
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
                    var getCommentReportsCondition = {
                        reporterId: $scope.personalProfile.id,
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

                    commentReportService.getCommentReports(getCommentReportsCondition)
                        .then(
                            function success(getCommentReportsResponse) {
                                var getCommentReportsResult = getCommentReportsResponse.data;

                                // Result is invalid.
                                if (!getCommentReportsResult) {
                                    fnCallback(items);
                                    return;
                                }

                                // Build items list.
                                items.data = getCommentReportsResult.records;
                                items.draw = draw;
                                items.recordsTotal = getCommentReportsResult.total;
                                items.recordsFiltered = getCommentReportsResult.total;
                                fnCallback(items);
                            },
                            function error() {
                                fnCallback(items);
                                return;
                            });
                })
        };

        // Columns list.
        $scope.dtColumns = {

            // Columns of post.
            posts: [
                // Title
                DTColumnBuilder.newColumn('title').withTitle($translate('Title')).notSortable(),
                // Type
                DTColumnBuilder.newColumn(null).withTitle($translate('Type')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.type) {
                            case postTypeConstant.private:
                                return '<span class="text-bold text-black">' + $translate.instant('Private') + '</span>';
                            default:
                                return '<span class="text-bold text-blue">' + $translate.instant('Public') + '</span>';
                        }
                    }
                ),
                // Status
                DTColumnBuilder.newColumn(null).withTitle($translate('Status')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.status) {
                            case postStatusConstant.disabled:
                                return '<span class="text-danger">' + $translate.instant('Disabled') + '</span>';
                            case postTypeConstant.available:
                                return '<span class="text-success">' + $translate.instant('Available') + '</span>';
                            default:
                                return '<span class="text-danger">' + $translate.instant('Deleted') + '</span>';

                        }
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
                        szUi += '<span>{{"Action" | translate}}</span> ';
                        szUi += '<span class="caret"></span>';
                        szUi += '</button>';
                        szUi += '<ul class="dropdown-menu" aria-labelledby="dropdownMenu1">';
                        szUi += '<li><a href="javscript:void(0);"><span class="fa fa-eye"></span>{{"View" | translate}}</a></li>';

                        // Viewer is the profile owner.
                        if (profile && profile.id === $scope.personalProfile.id) {
                            // Item is still available. Which means it can be deleted.
                            if (item.status === postStatusConstant.available)
                                szUi += '<li><a href="javscript:void(0);"><span class="fa fa-trash"></span> <b class="text-danger">{{"Delete" | translate}}</b> </a></li>';
                        }

                        szUi += '</ul>';
                        szUi += '</div>';
                        return szUi;
                    }
                )
            ],

            // List of comments.
            comments: [
                // Comment content
                DTColumnBuilder.newColumn('content').withClass('comment-content').withTitle($translate('Content')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return '<div class="truncate">' + item.content + '</div>';
                    }
                ),
                // Comment status
                DTColumnBuilder.newColumn(null).withTitle($translate('Status')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        switch (item.status) {
                            case commentStatusConstant.unavailable:
                                return $interpolate('<span class="text-bold text-danger">{{"Deleted" | translate}}</span>');
                            default:
                                return $interpolate('<span class="text-bold text-success">{{"Available" | translate}}</span>')
                        }
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
                        szUi += '<button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">';
                        szUi += $translate.instant('Action') + ' ';
                        szUi += '<span class="caret"></span>';
                        szUi += '</button>';
                        szUi += '<ul class="dropdown-menu" aria-labelledby="dropdownMenu1">';
                        szUi += '<li><a href="javscript:void(0);"><span class="fa fa-eye"></span> ' + $translate.instant('View post') + ' </a></li>';

                        // Viewer is the profile owner.
                        if (profile && profile.id === $scope.personalProfile.id) {
                            // If the item status is available. It can be deleted.
                            if (item.status === commentStatusConstant.available) {
                                var params = {
                                    commentId: item.id
                                };
                                szUi += $interpolate('<li><a href="javascript:void(0);" ng-click="fnDeleteComment({{commentId}})"><span class="fa fa-trash text-danger"></span>{{"Delete" | translate}}</a></li>')(params);
                            }
                        }
                        szUi += '</ul>';
                        szUi += '</div>';
                        return szUi;
                    }
                )
            ],

            // Comment reports list.
            postReports: [
                // Comment content
                DTColumnBuilder.newColumn(null).withClass('post-report-content').withTitle($translate('Content')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return '<div class="truncate">' + 'Post content' + '</div>';
                    }
                ),
                // Reason
                DTColumnBuilder.newColumn(null).withClass('post-report-reason').withTitle($translate('Reason')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return '<div class="truncate">' + 'Reason' + '</div>';
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
                        szUi += '<button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">';
                        szUi += $translate.instant('Action') + ' ';
                        szUi += '<span class="caret"></span>';
                        szUi += '</button>';
                        szUi += '<ul class="dropdown-menu" aria-labelledby="dropdownMenu1">';
                        szUi += '<li><a href="javscript:void(0);"><span class="fa fa-eye"></span> ' + $translate.instant('View post') + ' </a></li>';

                        // Viewer is the profile owner.
                        if (profile && profile.id === $scope.personalProfile.id) {
                            szUi += $interpolate('<li ng-click="fnDeletePostReport({{postId}})"><a href="javscript:void(0);"><span class="fa fa-trash text-bold text-danger"></span>{{"Delete" | translate}}</a></li>')({postId: item.postId});
                        }
                        szUi += '</ul>';
                        szUi += '</div>';
                        return szUi;
                    }
                )
            ],

            // Comment reports list.
            commentReports: [
                // Comment content
                DTColumnBuilder.newColumn(null).withClass('post-content').withTitle($translate('Post title')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return '<div class="truncate">' + '<a href="javascript:void(0);">' + 'Post title' + '</a>' + '</div>';
                    }
                ),
                // Comment content
                DTColumnBuilder.newColumn(null).withClass('comment-content').withTitle($translate('Comment content')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return '<a href="javascript:void(0);">' + 'Comment content' + '</a>';
                    }
                ),
                // Comment report reason
                DTColumnBuilder.newColumn(null).withClass('comment-content').withTitle($translate('Comment report reason')).notSortable().renderWith(
                    function (data, type, item, meta) {
                        return 'Comment report reason';
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
                        szUi += '<button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">';
                        szUi += $translate.instant('Action') + ' ';
                        szUi += '<span class="caret"></span>';
                        szUi += '</button>';
                        szUi += '<ul class="dropdown-menu" aria-labelledby="dropdownMenu1">';
                        szUi += '<li><a href="javascript:void(0);"><span class="fa fa-eye"></span> ' + $translate.instant('View post') + ' </a></li>';
                        szUi += '<li><a href="javascript:void(0);"><span class="fa fa-eye"></span> ' + $translate.instant('View comment') + ' </a></li>';
                        // Viewer is the profile owner.
                        if (profile && profile.id === $scope.personalProfile.id) {
                            //szUi += '<li ' + 'ng-click="fnClickDeleteCommentReport"' + '><a href="javascript:void(0);"><span class="fa fa-trash"></span> ' + $translate.instant('Delete') + ' </a></li>';
                            var szDelete = '<li ng-click="fnClickDeleteCommentReport({{commentId}}, {{reporterId}})"><a href="javascript:void(0);"><span class="fa fa-trash text-bold text-danger"></span>{{"Delete" | translate}}</a></li>';
                            szDelete = $interpolate(szDelete)({
                                commentId: item.commentId,
                                reporterId: $scope.personalProfile.id
                            });
                            szUi += szDelete;
                            //szUi += '<li ng-click="fnClickDeleteCommentReport"><a href="javascript:void(0);"><span class="fa fa-trash"></span> ' + $translate.instant('Delete') + ' </a></li>';
                        }

                        szUi += '</ul>';
                        szUi += '</div>';
                        return szUi;
                    }
                )
            ]
        };

        // Data-table instances list.
        $scope.dtInstances = {
            comments: {},
            postReports: {}
        };

        // File uploaders collection.
        $scope.fileUploader = {
            profileImageSelector: new FileUploader()
        };

        //#endregion

        //#region Methods

        /*
        * Function which is raised when component is initiated.
        * */
        $scope.fnInit = function () {

        };

        /*
        * Event which is fired when change password button is clicked.
        * */
        $scope.fnClickChangePassword = function () {

            // Not the profile owner.
            if (!profile || profile.id !== $scope.personalProfile.id)
                return;

            // Modal is valid. Close it first.
            if ($scope.modals.changePassword) {
                $scope.modals.changePassword.close();
                $scope.modals.changePassword = null;
            }

            $scope.modals.changePassword = $uibModal.open({
                templateUrl: 'change-password.html',
                scope: $scope,
                size: 'lg'
            });
        };

        /*
        * Event which is for submitting new password.
        * */
        $scope.fnSubmitPassword = function ($event, bIsFormValid) {

            // Prevent default behaviour.
            if ($event)
                $event.preventDefault();

            // Form is invalid.
            if (!bIsFormValid)
                return;

            // Submit information to server.
            userService.changePassword($scope.personalProfile.id, $scope.model.changePassword)
                .then(function (changePasswordResponse) {

                    // Close modal dialog.
                    if ($scope.modals.changePassword) {
                        $scope.modals.changePassword.dismiss();
                        $scope.modals.changePassword = null;
                    }

                    // Get result.
                    var changePasswordResult = changePasswordResponse.data;
                    if (!changePasswordResult) {
                        return;
                    }

                    // Display success message.
                    toastr.success($translate.instant('Changed password successfully'));

                    // As user is changing his/her own password. Obtain the access token returned by this api.
                    if (profile.id === $scope.personalProfile.id) {
                        var szAccessToken = changePasswordResult.accessToken;
                        authenticationService.initAuthenticationToken(szAccessToken);
                    }

                    // Reload the current state.
                    $state.reload();
                });
        };

        /*
        * Delete comment by using specific condition.
        * */
        $scope.fnDeleteComment = function (commentId) {
            // Display a confirmation message.
            $ngConfirm({
                title: $translate.instant('Confirmation'),
                content: '<b class="text-bold text-danger">' + $translate.instant('Are you sure to delete this comment ?') + '</b>',
                scope: $scope,
                buttons: {
                    ok: {
                        text: $translate.instant('OK'),
                        btnClass: 'btn btn-danger btn-flat',
                        action: function (scope, button) {
                            // Delete comment by searching for its id.
                            commentService.deleteComment(commentId)
                                .then(function () {

                                    // Display success message.
                                    toastr.success($translate.instant('Comment has been deleted successfully'));

                                    // Reload the data-table.
                                    $scope.dtInstances.comments.dataTable.fnDraw();
                                });
                        }
                    },
                    cancel:{
                        text: $translate.instant('Cancel'),
                        btnClass: 'btn btn-default btn-flat',
                        action: function (scope, button) {
                        }
                    }
                }
            });


        };

        /*
        * Function which is for deleting comment report.
        * */
        $scope.fnClickDeleteCommentReport = function (commentId, reporterId) {
            $ngConfirm({
                title: '',
                content: '<b class="text-danger">' + $translate.instant('Are you sure to delete this comment report ?') + '</b>',
                scope: $scope,
                buttons: {
                    ok: {
                        text: $translate.instant('OK'),
                        btnClass: 'btn btn-flat btn-danger',
                        action: function (scope, button) {
                            // Delete comment report.
                            return commentReportService.deleteCommentReport(commentId, reporterId)
                                .then(
                                    function success() {
                                        return true;
                                    },
                                    function error() {
                                        return true;
                                    }
                                );
                        }
                    },
                    cancel: {
                        text: $translate.instant('Cancel'),
                        btnClass: 'btn btn-flat btn-default',
                        action: function (scope, button) {
                        }
                    }
                }
            });
        };

        /*
        * Function to display change profile image modal dialog.
        * */
        $scope.fnChangeProfileImage = function () {
            $scope.modals.changeProfileImage = $uibModal.open({
                templateUrl: 'change-profile-avatar.html',
                scope: $scope,
                size: 'lg'
            });
        };

        /*
        * Cancel editing profile image to select another image.
        * */
        $scope.fnCancelEditProfileImage = function () {

            if ($scope.model.profile.originalImage) {
                // Reset original image and cropped image.
                $scope.model.profile.croppedImage = null;
                $scope.model.profile.originalImage = null;
                return;
            }

            if ($scope.modals.changeProfileImage) {
                $scope.modals.changeProfileImage.dismiss();
                $scope.modals.changeProfileImage = null;
            }
        };

        /*
        * Upload profile avatar to server.
        * */
        $scope.fnUploadProfileAvatar = function () {
            // Cropped image hasn't been defined.
            if (!$scope.model.profile.croppedImage)
                return;

            userService.uploadProfileAvatar($scope.model.profile.croppedImage)
                .then(function (uploadProfileAvatarResponse) {

                    // Dismiss the modal if it is available.
                    if ($scope.modals.changeProfileImage) {
                        $scope.modals.changeProfileImage.dismiss();
                        $scope.modals.changeProfileImage = null;
                    }

                    // Reload the current state.
                    $state.reload();
                });
        };

        /*
        * Delete a specific post report.
        * */
        $scope.fnDeletePostReport = function (postId) {

            $ngConfirm({
                title: $translate.instant('Confirmation'),
                content: '<b class="text-danger">' + $translate.instant('Are you sure to delete this post report ?') + '</b>',
                scope: $scope,
                buttons: {
                    ok: {
                        text: $translate.instant('OK'),
                        btnClass: 'btn btn-danger btn-flat',
                        action: function (scope, button) {
                            postReportService.deletePostReport(postId)
                                .then(function (deletePostReportId) {
                                    // Reload the post report data-table instant.
                                    $scope.dtInstances.postReports.dataTable.fnDraw();

                                    // Display success message.
                                    toastr.success($translate.instant('Post report has been deleted successfully'));
                                });
                        }
                    },
                    cancel: {
                        text: $translate.instant('Cancel'),
                        btnClass: 'btn btn-default btn-flat',
                        action: function ($scope, button) {

                        }
                    }
                }
            });


        };

        /*
        * Function which is raised when profile image is selected.
        * */
        $scope.fileUploader.profileImageSelector.onAfterAddingFile = function (fileItem) {
            var fileReader = new FileReader();
            fileReader.onload = function (evt) {
                $scope.$apply(function ($scope) {
                    $scope.model.profile.originalImage = evt.target.result;
                });
            };
            fileReader.readAsDataURL(fileItem._file);
        };

        /*
        * Function which is raised when profile image imported failingly.
        * */
        $scope.fileUploader.profileImageSelector.onWhenAddingFileFailed = function () {

            // Reset the selected image.
            $scope.model.profile.originalImage = null;
            $scope.model.profile.croppedImage = null;

            $ngConfirm({
                title: ' ',
                content: '<b class="text-danger text-bold">' + $translate.instant('Please select a valid image') + '</b>',
                scope: $scope,
                buttons: {
                    ok: {
                        text: $translate.instant('OK'),
                        btnClass: 'btn btn-default btn-flat',
                        action: function (scope, button) {
                        }
                    }
                }
            });
        };

        /*
        * Function which is raised when component is initiated successfully.
        * */
        $timeout(function () {
            $scope.fileUploader.profileImageSelector.filters.push({
                name: 'image-filter',
                fn: function (item /*{File|FileLikeObject}*/, options) {
                    return (item.type && item.type.indexOf('image') !== -1);
                }
            });
        });

        //#endregion
    });
};