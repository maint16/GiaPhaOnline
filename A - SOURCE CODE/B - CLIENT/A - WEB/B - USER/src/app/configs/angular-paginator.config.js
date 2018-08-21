module.exports = (ngModule) => {
    /*
    * AngularJS pager default configuration.
    * */
    ngModule
        .config((uibPaginationConfig) => {
            uibPaginationConfig.lastText = '>>';
            uibPaginationConfig.nextText = '>';
            uibPaginationConfig.previousText = '<';
            uibPaginationConfig.firstText = '<<';
            uibPaginationConfig.maxSize = 5;
        });
};