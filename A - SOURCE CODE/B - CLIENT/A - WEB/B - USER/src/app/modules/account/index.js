module.exports = function(ngModule){

    // Load controllers.
    require('./login/login.controller')(ngModule);
    require('./profile/profile.controller')(ngModule);

    // Load routes.
    require('./login/login.route')(ngModule);
    require('./profile/profile.route')(ngModule);
};
