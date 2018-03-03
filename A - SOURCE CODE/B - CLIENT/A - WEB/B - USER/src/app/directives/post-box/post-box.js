module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./post-box.html');

    // Directive declaration.
    ngModule
        .directive('postBox', function () {
            return {
                template: ngModuleHtmlTemplate,
                restrict: 'AE',
                transclude:{
                    postContent: '?postContent'
                },
                scope: {
                    ngPost: '=',
                    ngIsFollowingPost: '=',
                    ngIsCommentAvailable: '=',
                    ngIsCommentSubmissionAvailable: '=',
                    ngIsFollowPostAvailable: '=',
                    ngUsers: '=',
                    ngAudienceProfile: '=',

                    followingPost: '&',
                    followedPost: '&',
                    unfollowingPost: '&',
                    unfollowedPost: '&'
                },
                controller: function ($scope, urlStates, appSettings, postTypeConstant, taskStatusConstant, taskResultConstant,
                                      userService, commentService, followPostService) {

                    //#region Properties

                    // Constants reflection.
                    $scope.urlStates = urlStates;
                    $scope.appSettings = appSettings;

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
                                records: appSettings.pagination.comments
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

                                // Load owners information.
                                userService.loadUsers(userIds)
                                    .then(function (getUsersResponse) {
                                        var getUsersResult = getUsersResponse.data;
                                        if (!getUsersResult)
                                            return;

                                        // Get users list.
                                        var users = getUsersResult.records;
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
                        if (!$scope.post || !$scope.model.content || $scope.model.content.length < 1)
                            return;

                        // Initialize comment entity.
                        var comment = {
                            postId: $scope.post.id,
                            content: $scope.model.content
                        };

                        commentService.addComment(comment)
                            .then(function (addCommentResponse) {

                                // Clear input box.
                                $scope.model.content = null;

                                // Reload comment lists.
                                $scope.getComments();
                            });
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
                    $scope.fnGetPostTypeName = function(type){
                        switch (type){
                            case postTypeConstant.private:
                                return 'Private';
                            default:
                                return 'Public';
                        }
                    };

                    /*
                    * Toggle post follow.
                    * */
                    $scope.fnTogglePostFollow = function(){

                        // Post is being followed. Unfollow it.
                        if ($scope.ngIsFollowingPost){
                            // Raise the callback.
                            $scope.unfollowingPost({status: taskStatusConstant.beforeAction, result: taskResultConstant.success});
                            followPostService.unfollowPost($scope.ngPost.id)
                                .then(function(){
                                    // Raise the callback.
                                    $scope.unfollowedPost({id: $scope.ngPost.id, status: taskStatusConstant.afterAction, result: taskResultConstant.success});
                                })
                                .catch(function(){
                                    // Raise the callback.
                                    $scope.unfollowedPost({id: $scope.ngPost.id, status: taskStatusConstant.afterAction, result: taskResultConstant.fail});
                                });
                            return;
                        }

                        // Raise the callback.
                        $scope.followingPost({status: taskStatusConstant.beforeAction, result: taskResultConstant.success});
                        followPostService.followPost($scope.ngPost.id)
                            .then(function(){
                                // Raise the callback.
                                $scope.followedPost({id: $scope.ngPost.id, status: taskStatusConstant.afterAction, result: taskResultConstant.success});
                            })
                            .catch(function(){
                                // Raise the callback.
                                $scope.followedPost({id: $scope.ngPost.id, status: taskStatusConstant.afterAction, result: taskResultConstant.fail});
                            });
                    };

                    //#endregion
                }
            }
        });
};