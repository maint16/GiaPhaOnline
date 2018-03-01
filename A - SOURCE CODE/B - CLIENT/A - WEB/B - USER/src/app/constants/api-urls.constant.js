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
            facebookLogin: 'api/user/facebook-login',
            uploadProfileImage: 'api/user/upload-avatar'
        },

        category: {
            getCategories: 'api/category/search'
        },

        post: {
            getPosts: 'api/post/search'
        },

        postReport:{
            getPostReports: 'api/post-report/search',
            deletePostReport: 'api/post-report'
        },

        postCategorization:{
            getPostCategorization: 'api/categorization/search'
        },

        comment: {
            addComment: 'api/comment',
            getComments: 'api/comment/search',
            loadComments: 'api/comment/load-comments',
            deleteComment: 'api/comment/{id}'
        },

        commentReports:{
            getCommentReports: 'api/comment-report/search',
            deleteCommentReports: 'api/comment-report'
        },

        followPost: {
            getFollowPost: 'api/follow-post/search',
            loadFollowPosts: 'api/follow-post/load'
        },

        followingCategory:{
            getFollowingCategory: 'api/follow-category/search',
            followCategory: 'api/follow-category',
            unfollowCategory: 'api/follow-category'
        }

    });
};