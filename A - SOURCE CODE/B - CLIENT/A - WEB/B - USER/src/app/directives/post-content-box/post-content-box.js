module.exports = function (ngModule) {

    // Module template import.
    var ngModuleHtmlTemplate = require('./post-content-box.html');

    // Directive declaration.
    ngModule.directive('postContentBox', function () {
        return {
            template: ngModuleHtmlTemplate,
            restrict: 'AE',
            scope: {
                post: '=',
                postOwner: '=',
                audienceProfile: '='
            },
            controller: function ($scope, urlStates, appSettings, userService,
                                  commentService) {

                //#region Properties

                // Constants reflection.
                $scope.urlStates = urlStates;
                $scope.appSettings = appSettings;

                // Buffer data which is for client caching.
                $scope.buffer = {
                    users: {},
                    followingPost: {},
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
                    if (!$scope.post)
                        return;

                    // Clear buffer.
                    $scope.buffer.users = {};

                    // Reset comment results.
                    $scope.result.getComments.records = [];

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
                                .then(function(getUsersResponse){
                                    var getUsersResult = getUsersResponse.data;
                                    if (!getUsersResult)
                                        return;

                                    // Get users list.
                                    var users = getUsersResult.records;
                                    users.map(function(user){
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
                $scope.addComment = function($event){

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
                        .then(function(addCommentResponse){

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
                    // Reset pagination.
                    $scope.condition.getComments.pagination.page = 1;
                    $scope.getComments();
                };

                //#endregion
            }
        }
    });
};