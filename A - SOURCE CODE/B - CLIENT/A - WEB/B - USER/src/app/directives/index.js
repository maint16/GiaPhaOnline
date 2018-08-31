module.exports = (ngModule) => {
    const {NavigationBarDirective} = require('./navigation-bar');
    ngModule.directive('navigationBar', ($q, $compile) => new NavigationBarDirective($q, $compile));

    const {AppFooterDirective} = require('./app-footer');
    ngModule.directive('appFooter', ($q, $compile) => new AppFooterDirective($q, $compile));
};