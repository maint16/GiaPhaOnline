module.exports = function (ngModule) {
    ngModule.constant('apiUrls', {

        user: {
            getUser: 'api/account/{id}',
            getUsers: 'api/account/search',
            loadUsers: 'api/account/load-users',
            getPersonalProfile: 'api/user/personal-profile'
        },

        category: {
            getCategories: 'api/category/search'
        },

        post: {
            getPosts: 'api/post/search'
        },

        comment: {
            getComments: 'api/comment/search',
            loadComments: 'api/comment/load-comments'
        },

        followPost: {
            getFollowPost: 'api/follow-post/search',
            loadFollowPosts: 'api/follow-post/load-follow-post'
        }

    });
};