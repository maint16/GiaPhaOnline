module.exports = (ngModule) => {
    require('./authentication.service')(ngModule);
    require('./ui.service')(ngModule);
    require('./category-group.service')(ngModule);
    require('./category.service')(ngModule);
    require('./oauth.service')(ngModule);

    // Mocking api.
    require('./category-group.service')(ngModule);
    require('./mock/topic.service')(ngModule);
};