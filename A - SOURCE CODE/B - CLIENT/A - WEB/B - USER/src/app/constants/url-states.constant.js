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

        authorizedLayout: {
            name: 'authorized-layout'
        },

        unauthorizedLayout: {
            name: 'unauthorized-layout'
        }
    });
};