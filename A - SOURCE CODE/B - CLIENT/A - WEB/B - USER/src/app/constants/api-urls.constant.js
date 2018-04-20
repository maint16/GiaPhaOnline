module.exports = function (ngModule) {
    ngModule.constant('apiUrls', {

        user: {
            getUser: 'api/user/{id}',
            getUsers: 'api/user/search',
            loadUsers: 'api/user/load-users',
            getPersonalProfile: 'api/user/personal-profile/{id}',
            changePassword: 'api/user/change-password/{id}',
            basicLogin: 'api/user/basic-login',
            basisRegister: 'api/user/basic-register',
            googleLogin: 'api/user/google-login',
            facebookLogin: 'api/user/facebook-login',
            uploadProfileImage: 'api/user/upload-avatar',
            editUserStatus: 'api/user/status/{userId}'
        },

        category: {
            getCategories: 'api/category/search',
            addCategory: 'api/category'
        },

        post: {
            getPosts: 'api/post/search',
            addPost: 'api/post',
            loadPosts: 'api/post/load-posts',
            editPostStatus: 'api/post/status/{id}'
        },

        postReport:{
            getPostReports: 'api/post-report/search',
            deletePostReport: 'api/post-report'
        },

        postCategorization:{
            getPostCategorization: 'api/post-categorization/search'
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
            loadFollowPosts: 'api/follow-post/load',
            followPost: 'api/follow-post',
            unfollowPost: 'api/follow-post'
        },

        followingCategory:{
            getFollowingCategory: 'api/follow-category/search',
            followCategory: 'api/follow-category',
            unfollowCategory: 'api/follow-category'
        },

        postNotification:{
            getPostNotifications: 'api/post-notification/search'
        },

        pushNotification:{
            addDevice: 'api/push-notification/device'
        },


        realtime:{
            authorizePusher: 'api/realtime-connection/pusher/authorize'
        }

    });
};