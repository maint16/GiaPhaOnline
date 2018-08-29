module.exports = (ngModule) => {
    // Import routes.
    require('./main/main.route')(ngModule);

    const {TopicsModule} = require('./topics');
    ngModule.config(($stateProvider) => new TopicsModule($stateProvider));

    require('./topic/topic.route')(ngModule);
    require('./add-edit-topic/add-edit-topic.route')(ngModule);
};