module.exports = (ngModule) => {
    // Import routes.
    require('./main/main.route')(ngModule);
    require('./category-topics/category-topics.route')(ngModule);
    require('./topic/topic.route')(ngModule);
    require('./add-edit-topic/add-edit-topic.route')(ngModule);
};