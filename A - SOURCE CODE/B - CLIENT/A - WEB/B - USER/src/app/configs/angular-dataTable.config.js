module.exports = function(ngModule){
    /*
    * AngularJS datatable default configuration.
    * */
    ngModule.run(function(DTDefaultOptions, appSettings) {
        DTDefaultOptions.setDisplayLength(appSettings.pagination.default);
        DTDefaultOptions.setOption('responsive', true);
        DTDefaultOptions.setOption('bFilter', false);
        DTDefaultOptions.setOption('processing', true);
        DTDefaultOptions.setOption('serverSide', true);
        DTDefaultOptions.setOption('bLengthChange', false);
        DTDefaultOptions.setOption('bInfo', false);
        DTDefaultOptions.setOption('order', []);
        DTDefaultOptions.setOption('pagingType', 'full_numbers');
        DTDefaultOptions.setDOM('<"top"i>rt<"dt-center-pagination"flp><"clear">');
        DTDefaultOptions.setLanguage({
            'paginate':{
                'first': '&lt;&lt;',
                'previous': '&lt;',
                'next': '&gt;',
                'last': '&gt;&gt;'
            }
        });

    });
};