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
                records: [],
                total: 0
            }
        };

        // Data load condition
        $scope.loadDataCondition = {
            post: {
                title: null,
                pagination:{
                    page: 1,
                    records: appSettings.pagination.default
                }

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
        $scope.loadPosts = function (conditions) {
            var loadDataCondition = conditions;
            if (loadDataCondition == null)
                loadDataCondition = $scope.loadDataCondition.post;

            // Call api to load data.
            return postService.getPosts(loadDataCondition)
                .then(function (x) {
                    var result = x.data;
                    if (!result)
                        return null;

                    // Get posts list.
                    var posts = result.records;

                    // Posts list.
                    $scope.loadDataResult.posts = result;

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
        * Callback which is fired when advanced search button is clicked.
        * */
        $scope.clickAdvancedSearch = function(bUseAdvancedSearch){
            $scope.bUsingAdvancedSearch = bUseAdvancedSearch;
        };

        /*
        * Called when user clicks on a page of category posts.
        * */
        $scope.clickChangePage = function(){

            // Clear buffers.
            $scope.buffer.users = {};
            $scope.buffer.comments = {};
            $scope.buffer.postComments = {};

            // Load posts.
            $scope.loadPosts($scope.loadDataCondition.post, true);
        };

        /*
        * Event is fired when basic search is used.
        * */
        $scope.fnBasicSearch = function($event){

            // Prevent default behaviour of control submission.
            $event.preventDefault();

            // Reset search condition.
            $scope.loadDataCondition.post.pagination.page = 1;

            // Search for posts.
            $scope.loadPosts($scope.loadDataCondition.post);
        };

        /*
        * Event which is fired when advanced search function is used.
        * */
        $scope.fnAdvanceSearch = function($event){

            // Prevent default behaviour.
            $event.preventDefault();

            // Reset search pagination.
            $scope.loadDataCondition.post.pagination.page = 1;

            // Search for posts.
            $scope.loadPosts($scope.loadDataCondition.post);
        };

        /*
        * Called when controller has been initialized.
        * */
        $timeout(function () {
            // Clear buffer data.
            $scope.buffer.users = {};
            $scope.buffer.followingPosts = {};

            $scope.loadPosts(null);
        });

        //#endregion
    });
};