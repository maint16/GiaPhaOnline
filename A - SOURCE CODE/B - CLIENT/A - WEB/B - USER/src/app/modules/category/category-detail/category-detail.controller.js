module.exports = function (ngModule) {
    ngModule.controller('categoryDetailController', function ($scope, $timeout,
                                                              profile,
                                                              appSettingConstant, urlStates, details, taskStatusConstant, taskResultConstant,
                                                              $uibModal, toastr, $translate,
                                                              postService, followPostService, commonService,
                                                              commentService, userService, followCategoryService) {

        //#region Properties

        // Constant reflection.
        $scope.appSettingConstant = appSettingConstant;
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

        // Collection of data which are splitted into chunks.
        $scope.chunks = {
            posts: []
        };

        // Data load condition
        $scope.loadDataCondition = {
            post: {
                title: null,
                pagination: {
                    page: 1,
                    records: appSettingConstant.pagination.default
                }

            },
            comment: {
                page: 1,
                records: appSettingConstant.pagination.default
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
                .then(function (getPostsResponse) {

                    // Initialize default items.
                    var defaultResponse = {
                        records: [],
                        total: 0
                    };

                    // Get result from response.
                    var getPostsResult = getPostsResponse.data;
                    if (!getPostsResult) {
                        $scope.loadDataResult.posts = defaultResponse;
                        throw 'Cannot get posts list.'
                    }

                    // No post has been found.
                    var posts = getPostsResult.records;
                    if (!posts || posts.length < 1) {
                        $scope.loadDataResult.posts = defaultResponse;
                        throw 'Cannot get posts list.'
                    }

                    return getPostsResult;
                })
                // Load following posts.
                .then(function (getPostsResult) {

                    // Build a list of promises which should be resolved
                    var promises = [];

                    // Get posts list.
                    var posts = getPostsResult.records;

                    //#region Get user list

                    // Get users list.
                    var userIds = posts
                        .filter(function (post) {
                            return !$scope.buffer.users[post.ownerId];
                        }).map(function (post) {
                            return post.ownerId;
                        });

                    // Load users list.
                    var loadUsersPromises = $scope.loadUsers(userIds);
                    if (loadUsersPromises && loadUsersPromises.length > 0)
                        promises = promises.concat(loadUsersPromises);

                    //#endregion

                    //#region Get post list

                    var posts = getPostsResult.records;

                    // List of post ids which must be loaded.
                    var postIds = [];
                    angular.forEach(posts, function(post){
                        if ($scope.buffer.followingPosts[post.id])
                            return;

                        postIds.push(post.id);

                        // Find maximum length of post.
                        var iMaximumPostLength = appSettingConstant.maxLength.categoryDetailPostBody;

                        // Truncate post.
                        var szBody = post.body;
                        if (szBody && szBody.length > iMaximumPostLength)
                            post.body = post.body.substring(0, 250) + ' ...';
                    });

                    // Load following post promises.
                    var loadFollowingPostsPromises = $scope.loadFollowingPosts(postIds);
                    if (loadFollowingPostsPromises && loadFollowingPostsPromises.length > 0)
                        promises = promises.concat(loadFollowingPostsPromises);

                    //#endregion

                    return Promise.all(promises)
                        .then(function () {
                            return getPostsResult;
                        })
                })
                .then(function (getPostsResult) {
                    // Posts list.
                    $scope.loadDataResult.posts = getPostsResult;
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

            var loadUsersPromise = userService.loadUsers(getUsersCondition)
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

            return [loadUsersPromise];
        };

        /*
        * Get following posts list.
        * */
        $scope.loadFollowingPosts = function (postIds) {
            var loadFollowingPostsPromise = followPostService.loadFollowPosts(postIds)
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

            return [loadFollowingPostsPromise];
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
                .then(function (addPostResponse) {

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