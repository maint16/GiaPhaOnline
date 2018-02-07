module.exports = function(ngModule){
    // Controllers import.
    require('./navigation-bar/navigation-bar')(ngModule);
    require('./side-bar/side-bar')(ngModule);
    require('./ui-view-css.directive')(ngModule);
    require('./post-search/post-search')(ngModule);
    require('./user-picker/user-picker')(ngModule);
};