module.exports = (ngModule) => {
    const {BlockUiConfiguration} = require('./block-ui.configuration');
    ngModule.config((blockUIConfig) => new BlockUiConfiguration(blockUIConfig));

    const {LocalStorageConfiguration} = require('./local-storage.configuration');
    ngModule.config((localStorageServiceProvider) => new LocalStorageConfiguration(localStorageServiceProvider));

    const {PaginationConfiguration} = require('./pagination.configuration');
    ngModule.config((uibPaginationConfig) => new PaginationConfiguration(uibPaginationConfig));

    const {ReCaptchaConfiguration} = require('./re-captcha.configuration');
    ngModule.config((appSettingConstant, vcRecaptchaServiceProvider) => new ReCaptchaConfiguration(appSettingConstant, vcRecaptchaServiceProvider));
};