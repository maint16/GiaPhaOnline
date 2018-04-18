module.exports = function(ngModule){
    ngModule.config(function (blockUIConfig) {
        // BlockUI configuration.
        blockUIConfig.autoInjectBodyBlock = false;
        blockUIConfig.templateUrl = 'main-block-ui.html';
    });
};