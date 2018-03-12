module.exports = function(ngModule){
    // Import controllers.
    require('./master-layout/master-layout.controller')(ngModule);
    require('./user-management/user-management.controller')(ngModule);
    require('./category-management/category-management.controller')(ngModule);

    // Import routes.
    require('./master-layout/master-layout.route')(ngModule);
    require('./user-management/user-management.route')(ngModule);
    require('./category-management/category-management.route')(ngModule);
};