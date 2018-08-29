module.exports = (ngModule) => {
    // Import routes.
    require('./main/main.route')(ngModule);

    const {TopicsModule} = require('./topics');
    ngModule.config(($stateProvider) => new TopicsModule($stateProvider));

    const {TopicModule} = require('./topic');
    ngModule.config(($stateProvider) => new TopicModule($stateProvider));

    require('./add-edit-topic/add-edit-topic.route')(ngModule);
};