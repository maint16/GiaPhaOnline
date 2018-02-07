module.exports = function(ngModule){

    // Import controllers.
    require('./category-detail/category-detail.controller')(ngModule);

    // Import routes.
    require('./category-detail/category-detail.route')(ngModule);
};