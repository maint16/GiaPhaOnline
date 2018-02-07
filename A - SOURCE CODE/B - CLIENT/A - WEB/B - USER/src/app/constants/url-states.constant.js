module.exports = function (ngModule) {

    /*
    * Application constants declaration.
    * */
    ngModule.constant('urlStates', {
        login:{
            url: '/login',
            name: 'login'
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