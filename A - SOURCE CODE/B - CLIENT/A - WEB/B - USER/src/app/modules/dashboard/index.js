module.exports = (ngModule) => {
    // Import routes.
    const {MainModule} = require('./main');
    ngModule.config(($stateProvider) => new MainModule($stateProvider));

    const {TopicsModule} = require('./topics');
    ngModule.config(($stateProvider) => new TopicsModule($stateProvider));

    const {TopicModule} = require('./topic');
    ngModule.config(($stateProvider) => new TopicModule($stateProvider));

    const {AddEditTopicModule} = require('./add-edit-topic');
    ngModule.config(($stateProvider) => new AddEditTopicModule($stateProvider));
};