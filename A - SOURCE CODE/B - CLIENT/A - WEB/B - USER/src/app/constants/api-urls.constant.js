module.exports = function (ngModule) {
    ngModule.constant('apiUrls', {

        user: {
            getUser: 'api/account/{id}',
            getUsers: 'api/account/search',
            loadUsers: 'api/account/load-users',
            getPersonalProfile: 'api/user/personal-profile',
            basicLogin: 'api/user/login'
        },

        category: {
            getCategories: 'api/category/search'
        },

        post: {
            getPosts: 'api/post/search'
        },

        postCategorization:{
            getPostCategorization: 'api/categorization/search'
        },

        comment: {
            addComment: 'api/comment',
            getComments: 'api/comment/search',
            loadComments: 'api/comment/load-comments'
        },

        followPost: {
            getFollowPost: 'api/follow-post/search',
            loadFollowPosts: 'api/follow-post/load'
        },

        followingCategory:{
            getFollowingCategory: 'api/follow-category/search'
        }

    });
};