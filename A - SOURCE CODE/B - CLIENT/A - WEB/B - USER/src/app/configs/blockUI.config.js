module.exports = (ngModule) => {
    ngModule.config((blockUIConfig => {
        // BlockUI configuration.
        blockUIConfig.autoInjectBodyBlock = false;
        blockUIConfig.templateUrl = 'main-block-ui.html';
    }));
};