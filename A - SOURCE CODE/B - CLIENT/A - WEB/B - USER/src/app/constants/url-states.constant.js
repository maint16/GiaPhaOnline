module.exports = function (ngModule) {

    /*
    * Application constants declaration.
    * */
    ngModule.constant('urlStates', {

        user:{
            // Login state.
            login:{
                name: 'login',
                url: '/login'
            },

            googleLogin:{
                name: 'google-login',
                url: '/google-login/:code'
            },

            // Profile state
            profile:{
                name: 'personal-profile',
                url: '/profile/:profileId'
            }
        },

        dashboard: {
            url: '/dashboard',
            name: 'dashboard'
        },

        category:{
          postListing:{
              url: '/category/:categoryId',
              name: 'category-detail'
          }
        },

        controlPanel:{
            masterLayout: {
                name: 'control-panel-master-layout',
                url: '/control-panel'
            },

            userManagement:{
                name: 'user-management',
                url: '/user-management'
            },

            categoryManagement:{
                name: 'category-management',
                url: '/category-management'
            }
        },

        authorizedLayout: {
            name: 'authorized-layout'
        },

        unauthorizedLayout: {
            name: 'unauthorized-layout'
        }
    });
};