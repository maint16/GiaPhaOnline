module.exports = function (ngModule) {
    ngModule.controller('categoryDetailController', function ($scope, $timeout,
                                                              profile,
                                                              appSettings, urlStates, details, taskStatusConstant, taskResultConstant,
                                                              $uibModal, toastr, $translate,
                                                              postService, followPostService,
                                                              commentService, userService, followCategoryService) {

        //#region Properties

        // Constant reflection.
        $scope.appSettings = appSettings;
        $scope.urlStates = urlStates;

        // Resolver reflection.
        $scope.profile = profile;

        // Service reflection.
        $scope.userService = userService;

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

        /*
        * List of modals dialog.
        * */
        $scope.modals = {
            postDetails: null,
            addPost: null
        };

        // Resolver reflection.
        $scope.bIsFollowingCategory = details.bIsFollowingCategory;
        $scope.category = details.category;

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
                pagination: {
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

        // Information of post detail box.
        $scope.postDetailBox = {
            post: null,
            postOwner: null
        };

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

            // Build users loading condition.
            var getUsersCondition = {
                ids: ids
            };

            userService.loadUsers(getUsersCondition)
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
                });
        };

        /*
        * Get following posts list.
        * */
        $scope.loadFollowingPosts = function (postIds) {
            followPostService.loadFollowPosts(postIds)
                .then(function (loadFollowPostsResponse) {

                    var loadFollowPostsResult = loadFollowPostsResponse.data;
                    if (!loadFollowPostsResult)
                        return;

                    var followPosts = loadFollowPostsResult.records;
                    followPosts.map(function (followPost) {
                        if ($scope.buffer.followingPosts[followPost.postId])
                            return followPost;

                        $scope.buffer.followingPosts[followPost.postId] = true;
                        return followPost;
                    });
                });
        };

        /*
        * Callback which is fired when advanced search button is clicked.
        * */
        $scope.clickAdvancedSearch = function (bUseAdvancedSearch) {
            $scope.bUsingAdvancedSearch = bUseAdvancedSearch;
        };

        /*
        * Called when user clicks on a page of category posts.
        * */
        $scope.clickChangePage = function () {

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
        $scope.fnBasicSearch = function ($event) {

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
        $scope.fnAdvanceSearch = function ($event) {

            // Prevent default behaviour.
            $event.preventDefault();

            // Reset search pagination.
            $scope.loadDataCondition.post.pagination.page = 1;

            // Search for posts.
            $scope.loadPosts($scope.loadDataCondition.post);
        };

        /*
        * Called when a post is selected.
        * */
        $scope.fnSelectPost = function (post) {
            $scope.postDetailBox.post = post;
            $scope.postDetailBox.postOwner = $scope.buffer.users[post.ownerId];

            // Display modal dialog.
            $scope.modals.postDetails = $uibModal.open({
                templateUrl: 'post-details.html',
                scope: $scope,
                size: 'lg'
            });
        };

        /*
        * Cancel adding post.
        * */
        $scope.fnCancelAddingPost = function () {
            // Modal has been initialized before. Dismiss it.
            if ($scope.modals.addPost) {
                $scope.modals.addPost.dismiss();
                $scope.modals.addPost = null;
            }
        };

        /*
        * Event which is fired when add post button is clicked.
        * */
        $scope.fnClickAddPost = function () {
            // Display modal dialog.
            $scope.modals.addPost = $uibModal.open({
                templateUrl: 'post-initiator.html',
                scope: $scope,
                size: 'lg'
            });
        };

        /*
        * Confirm to add a post.
        * */
        $scope.fnAddPost = function (post) {
            // Add post to system.
            postService.addPost(post)
                .then(function(addPostResponse){

                    // Dismiss the modal.
                    if ($scope.modals.addPost)
                        $scope.modals.addPost.dismiss();

                    // Reload the post list.
                    $scope.loadPosts(null);
                });
        };

        /*
        * Start following category.
        * */
        $scope.fnFollowCategory = function (categoryId) {

            // Category has been followed.
            if ($scope.bIsFollowingCategory)
                return;

            // Follow the category by using api call.
            followCategoryService.followCategory(categoryId)
                .then(function (followCategoryResponse) {
                    toastr.success($translate.instant('Start following this category'));

                    // Change category to be followed.
                    $scope.bIsFollowingCategory = true;
                });
        };

        /*
        * Unfollow category.
        * */
        $scope.fnUnfollowCategory = function (categoryId) {
            // Category has been unfollowed before.
            if (!$scope.bIsFollowingCategory) {
                return;
            }

            // Stop following the category.
            followCategoryService.unfollowCategory(categoryId)
                .then(function (unfollowCategoryResponse) {
                    toastr.success($translate.instant('Stop following this category'));

                    // Mark the category to be unfollowed.
                    $scope.bIsFollowingCategory = false;
                });
        };

        /*
        * Raised when post is followed.
        * */
        $scope.fnFollowedPost = function (id, status, result) {
            // Task is not complete.
            if (status !== taskStatusConstant.afterAction)
                return;

            if (result !== taskResultConstant.success)
                return;

            $scope.buffer.followingPosts[id] = true;
        };

        /*
        * Raised when post is unfollowed.
        * */
        $scope.fnUnfollowedPost = function (id, status, result) {
            // Task is not complete.
            if (status !== taskStatusConstant.afterAction)
                return;

            if (result !== taskResultConstant.success)
                return;

            $scope.buffer.followingPosts[id] = false;
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