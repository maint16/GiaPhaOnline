module.exports = (ngModule) => {
    // Load routes.
    require('./login/login.route')(ngModule);
    require('./profile/profile.route')(ngModule);
    require('./register/register.route')(ngModule);
    require('./forgot-password/forgot-password.route')(ngModule);
};
