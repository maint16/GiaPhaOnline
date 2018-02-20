module.exports = function (ngModule) {
    ngModule.constant('apiUrls', {

        user: {
            getUser: 'api/account/{id}',
            getUsers: 'api/account/search',
            loadUsers: 'api/account/load-users',
            getPersonalProfile: 'api/user/personal-profile/{id}',
            changePassword: 'api/user/change-password/{id}',
            basicLogin: 'api/user/login',
            googleLogin: 'api/user/google-login',
            facebookLogin: 'api/user/facebook-login'
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