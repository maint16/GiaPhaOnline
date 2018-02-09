module.exports = function (ngModule) {
    ngModule.controller('categoryDetailController', function ($scope, $timeout,
                                                              profile,
                                                              appSettings, followCategory,
                                                              postService, followPostService, commentService, userService) {

        //#region Properties

        // Constant reflection.
        $scope.appSettings = appSettings;

        // Buffer data which is for information binding and local caching.
        $scope.buffer = {
            postComments: {},
            comments: {},
            users: {},
            followingPosts: {}
        };

        // Model which is for model binding.
        $scope.model = {
            initialPostContent: null
        };

        // Resolver reflection.
        $scope.followCategory = followCategory;

        // Data which obtained from api service.
        $scope.loadDataResult = {
            posts: {
                data: [],
                total: 0,
                remain: 0
            }
        };

        // Data load condition
        $scope.loadDataCondition = {
            post: {
                page: 1,
                records: appSettings.pagination.default
            },
            comment: {
                page: 1,
                records: appSettings.pagination.default
            }
        };

        // Whether advanced search is being used or not.
        $scope.bUsingAdvancedSearch = false;

        //#endregion

        //#region Methods

        /*
        * Load posts by using specific conditions.
        * */
        $scope.loadPosts = function (conditions, bClearBufferData) {
            var loadDataCondition = conditions;
            if (loadDataCondition == null)
                loadDataCondition = $scope.loadDataCondition.post;

            // Clear buffer data.
            if (bClearBufferData) {
                $scope.loadDataResult.posts = {
                    data: [],
                    total: 0,
                    remain: 0
                };
            }

            // Call api to load data.
            return postService.getPosts(loadDataCondition)
                .then(function (x) {
                    var result = x.data;
                    if (!result)
                        return null;

                    // Get posts list.
                    var posts = result.records;

                    debugger;
                    // Posts list.
                    $scope.loadDataResult.posts.data = $scope.loadDataResult.posts.data.concat(posts);
                    $scope.loadDataResult.posts.total = result.total;

                    // Get users list.
                    var userIds = posts
                        .filter(function (post) {
                            return !$scope.buffer.users[post.ownerId];
                        }).map(function (post) {
                            return post.ownerId;
                        });

                    // Load users list.
                    $scope.loadUsers(userIds);

                    return result;
                })

                // Load following posts.
                .then(function (getPostsResult) {

                    // Get post result.
                    if (!getPostsResult)
                        return null;

                    var posts = getPostsResult.records;
                    var postIds = posts
                        .filter(function (post) {
                            return !$scope.buffer.followingPosts[post.id]
                        })
                        .map(function (post) {
                            return post.id;
                        });

                    $scope.loadFollowingPosts(postIds);
                });
        };

        /*
        * Load comments by using specific conditions.
        * */
        $scope.loadComments = function (conditions, bClearBufferData) {
            var loadDataCondition = conditions;
            if (loadDataCondition == null)
                loadDataCondition = $scope.loadDataCondition.comment;

            // Clear buffer data.
            if (bClearBufferData)
                $scope.buffer.comments = {};

            // Call api to load data.
            return commentService.getComments(loadDataCondition)
                .then(function (x) {
                    var result = x.data;
                    if (!result)
                        return null;

                    // Get posts list.
                    var comments = result.records;

                    // Initialize users to load api.
                    var userIds = [];

                    // Go through every posts, check the cache and import the post to cache as its not available.
                    angular.forEach(comments, function (comment, iterator) {
                        // Not in cache.
                        if (!$scope.buffer.comments[comment.id]) {
                            $scope.buffer.comments[comment.id] = comment;

                            // Not in post comments cache.
                            if (!$scope.buffer.postComments[comment.postId]) {
                                $scope.buffer.postComments[comment.postId] = [];
                            }

                            // Attach post comment.
                            $scope.buffer.postComments[comment.postId].push(comment.id);

                            if (!$scope.buffer.users[comment.ownerId])
                                userIds.push(comment.ownerId);

                        }
                    });

                    // Load users list.
                    if (userIds && userIds.length > 0)
                        $scope.loadUsers(userIds);
                    return result;
                });
        };

        /*
        * Load users
        * */
        $scope.loadUsers = function (ids) {
            userService.loadUsers(ids)
                .then(function (loadUserResponse) {
                    var loadUserResult = loadUserResponse.data;
                    if (!loadUserResult)
                        return null;

                    var users = loadUserResult.records;
                    if (!users || users.length < 1)
                        return null;

                    angular.forEach(users, function (user, iterator) {

                        if ($scope.buffer.users[user.id])
                            return;

                        $scope.buffer.users[user.id] = user;
                    });
                })
        };

        /*
        * Load post comments.
        * */
        $scope.loadPostComments = function (post, initialLoad) {
            var conditions = {
                postId: post.id,
                pagination: {
                    page: 1,
                    records: appSettings.pagination.default
                }
            };

            $scope.loadComments(conditions, null);
        };

        /*
        * Get following posts list.
        * */
        $scope.loadFollowingPosts = function(postIds){
            followPostService.loadFollowPosts(postIds)
                .then(function(loadFollowPostsResponse){

                    var loadFollowPostsResult = loadFollowPostsResponse.data;
                    if (!loadFollowPostsResult)
                        return;

                    var followPosts = loadFollowPostsResult.records;
                    followPosts.map(function(followPost){
                        if ($scope.buffer.followingPosts[followPost.postId])
                            return followPost;

                        $scope.buffer.followingPosts[followPost.postId] = followPost;
                        return followPost;
                    });
                });
        };

        /*
        * Called when controller has been initialized.
        * */
        $timeout(function () {
            // Clear buffer data.
            $scope.buffer.postComments = {};
            $scope.buffer.comments = {};
            $scope.buffer.users = {};
            $scope.buffer.followingPosts = {};

            $scope.loadPosts(null, false);
        });

        /*
        * Callback which is fired when advanced search button is clicked.
        * */
        $scope.clickAdvancedSearch = function(bUseAdvancedSearch){
            $scope.bUsingAdvancedSearch = bUseAdvancedSearch;
        }

        //#endregion
    });
};