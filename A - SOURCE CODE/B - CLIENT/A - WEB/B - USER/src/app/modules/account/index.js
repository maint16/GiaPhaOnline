module.exports = (ngModule) => {
    // Load routes.
    const {LoginModule} = require('./login');
    ngModule.config(($stateProvider) => new LoginModule($stateProvider));

    const {ProfileModule} = require('./profile');
    ngModule.config(($stateProvider) => new ProfileModule($stateProvider));

    const {BasicRegisterModule} = require('./basic-register');
    ngModule.config(($stateProvider) => new BasicRegisterModule($stateProvider));

    const {ForgotPasswordModule} = require('./forgot-password');
    ngModule.config(($stateProvider) => new ForgotPasswordModule($stateProvider));
};
