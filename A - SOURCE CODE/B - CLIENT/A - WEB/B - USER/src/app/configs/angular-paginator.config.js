module.exports = function(ngModule){
    /*
    * AngularJS paginator default configuration.
    * */
    ngModule.config(function (uibPaginationConfig) {
        uibPaginationConfig.lastText = '>>';
        uibPaginationConfig.nextText = '>';
        uibPaginationConfig.previousText = '<';
        uibPaginationConfig.firstText = '<<';
        uibPaginationConfig.maxSize = 5;
    });
};