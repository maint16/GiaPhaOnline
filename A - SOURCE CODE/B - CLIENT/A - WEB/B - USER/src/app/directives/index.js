module.exports = function(ngModule){
    // Controllers import.
    require('./navigation-bar/navigation-bar')(ngModule);
    require('./side-bar/side-bar')(ngModule);
    require('./ui-view-css.directive')(ngModule);
    require('./post-search/post-search')(ngModule);
    require('./user-picker/user-picker')(ngModule);
    require('./post-content-box/post-content-box')(ngModule);
    require('./post-initiator/post-initiator')(ngModule);
    require('./basic-login-box/basic-login-box')(ngModule);
};