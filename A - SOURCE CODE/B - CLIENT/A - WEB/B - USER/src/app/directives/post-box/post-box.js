module.exports = (ngModule) => {

    // Directive declaration.
    ngModule
        .directive('postBox', () => {
            return {
                compile: () => {
                    let pGetTemplatePromise = $q((resolve) => {
                        require.ensure([], () => resolve(require('./post-box.html')));
                    });

                    return (scope, element) => {
                        pGetTemplatePromise
                            .then((htmlTemplate) => {
                                element.html(htmlTemplate);
                                $compile(element.contents())(scope)
                            });
                    };
                },
                restrict: 'AE',
                transclude: {
                    postContent: '?postContent',
                    postBoxFooter: '?postBoxFooter'
                },
                scope: {
                    ngPost: '=',
                    ngIsFollowingPost: '=',
                    ngIsCommentAvailable: '=',
                    ngIsCommentSubmissionAvailable: '=',
                    ngIsFollowPostAvailable: '=',
                    ngUsers: '=',
                    ngAudienceProfile: '=',

                    ngSubmitComment: '&',
                    ngClickFollowPost: '&'

                },
                controller: ($scope, urlStates, appSettingConstant, itemStatusConstant,
                             postTypeConstant, taskStatusConstant, taskResultConstant,
                             userService, commentService, followPostService) => {

                    //#region Properties

                    // Constants reflection.
                    $scope.urlStates = urlStates;
                    $scope.appSettingConstant = appSettingConstant;
                    $scope.itemStatusConstant = itemStatusConstant;

                    // Buffer data which is for client caching.
                    $scope.buffer = {
                        users: {},
                        followingPost: {},
                        followerCounter: null,
                        postOwner: null
                    };

                    // Conditions to get data.
                    $scope.condition = {
                        getComments: {
                            postId: 0,
                            pagination: {
                                page: 1,
                                records: appSettingConstant.pagination.comments
                            }
                        }
                    };

                    // Result obtained from api service.
                    $scope.result = {
                        getComments: {
                            records: [],
                            total: 0
                        }
                    };

                    /*
                    * Model which is for information binding.
                    * */
                    $scope.model = {
                        content: null
                    };

                    //#endregion

                    //#region Methods

                    /*
                    * Load post comments.
                    * */
                    $scope.getComments = function () {

                        // Post information is invalid.
                        if (!$scope.ngPost)
                            return;

                        // Clear buffer.
                        $scope.buffer.users = $scope.ngUsers;

                        // Reset comment results.
                        $scope.result.getComments.records = [];

                        // Update post id in search condition.
                        $scope.condition.getComments.postId = $scope.ngPost.id;

                        // Get comments list.
                        commentService.getComments($scope.condition.getComments)
                            .then(function (getCommentsResponse) {
                                var getCommentsResult = getCommentsResponse.data;
                                if (!getCommentsResult) {
                                    throw 'Cannot get comments list';
                                }

                                return getCommentsResult;
                            })
                            .then(function (getCommentsResult) {

                                // Get comments list.
                                var comments = getCommentsResult.records;
                                var userIds = comments
                                    .filter(function (comment) {
                                        return !$scope.buffer.users[comment.ownerId];
                                    })
                                    .map(function (comment) {
                                        return comment.ownerId;
                                    });

                                var getUsersCondition = {
                                    userIds: userIds
                                };

                                // Load owners information.
                                userService.loadUsers(getUsersCondition)
                                    .then(function (getUsersResponse) {
                                        var getUsersResult = getUsersResponse.data;
                                        if (!getUsersResult)
                                            return;

                                        // Get users list.
                                        var users = getUsersResult.records;
                                        if (!users || users.length < 1) {
                                            $scope.result.getComments = getCommentsResult;
                                            return;
                                        }

                                        users.map(function (user) {
                                            if ($scope.buffer.users[user.id])
                                                return user;

                                            $scope.buffer.users[user.id] = user;
                                        });

                                        $scope.result.getComments = getCommentsResult;

                                    });
                            });
                    };

                    /*
                    * Submit post comment.
                    * */
                    $scope.addComment = function ($event) {

                        // Prevent default behaviour as event is valid.
                        if ($event)
                            $event.preventDefault();

                        // Post is invalid.
                        var post = $scope.ngPost;
                        if (!post || !$scope.model.content || $scope.model.content.length < 1)
                            return;

                        // Initialize comment entity.
                        var comment = {
                            postId: post.id,
                            content: $scope.model.content
                        };

                        // Raise comment submission event.
                        if ($scope.ngSubmitComment({comment: comment})) {
                            // Clear input box.
                            $scope.model.content = null;

                            // TODO: Concat comment, instead of reloading 'em.
                            // Reload comment lists.
                            $scope.getComments();
                        }
                    };

                    /*
                    * Called when component is initialized.
                    * */
                    $scope.init = function () {

                        // Load buffer.
                        if ($scope.ngUsers)
                            $scope.buffer.users = $scope.ngUsers;

                        // Reset pagination.
                        $scope.condition.getComments.pagination.page = 1;

                        if ($scope.ngIsCommentAvailable)
                            $scope.getComments();
                    };

                    /*
                    * Get name base on status.
                    * */
                    $scope.fnGetPostTypeName = function (type) {
                        switch (type) {
                            case postTypeConstant.private:
                                return 'Private';
                            default:
                                return 'Public';
                        }
                    };

                    /*
                    * Toggle post follow.
                    * */
                    $scope.fnTogglePostFollow = function () {
                        // Post is being followed. Unfollow it.
                        if ($scope.ngIsFollowingPost) {
                            // Raise click follow post event.
                            $scope.ngClickFollowPost({post: $scope.ngPost, action: itemStatusConstant.deleted});
                            return;
                        }

                        $scope.ngClickFollowPost({post: $scope.ngPost, action: itemStatusConstant.available});
                    };

                    /*
                    * Check whether user is able to see comments or not.
                    * */
                    $scope.bIsAbleToSeeComment = function () {

                        // Comment is not available.
                        if (!$scope.ngIsCommentAvailable)
                            return false;

                        // No comment is in list.
                        var comments = $scope.result.getComments.records;
                        if (!comments)
                            return false;

                        return comments.length > 0;
                    };

                    /*
                    * Check whether follow button is available on post or not.
                    * */
                    $scope.bIsAbleToFollowPost = function () {

                        // No profile is attached.
                        if (!$scope.ngAudienceProfile)
                            return false;

                        // Post follow status is not specified.
                        if (!$scope.ngIsFollowPostAvailable)
                            return false;

                        if ($scope.ngPost.status !== itemStatusConstant.available)
                            return false;

                        return true;

                    };

                    //#endregion
                }
            }
        });
};