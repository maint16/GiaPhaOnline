module.exports = (ngModule) => {
    const {NavigationBarDirective} = require('./navigation-bar');
    ngModule.directive('navigationBar', ($q, $compile) => new NavigationBarDirective($q, $compile));

    require('./side-bar/side-bar')(ngModule);
    require('./bottom-footer/bottom-footer')(ngModule);
};