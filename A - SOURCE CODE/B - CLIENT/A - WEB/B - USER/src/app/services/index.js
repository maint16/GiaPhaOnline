module.exports = (ngModule) => {

    const {UiService} = require('./ui.service');
    ngModule.service('$ui', UiService);

    const {CategoryGroupService} = require('./category-group.service');
    ngModule.service('$categoryGroup', CategoryGroupService);

    const {CategoryService} = require('./category.service');
    ngModule.service('$category', CategoryService);

    require('./oauth.service')(ngModule);

    const {TopicService} = require('./topic.service');
    ngModule.service('$topic', TopicService);

    const {ReplyService} = require('./reply.service');
    ngModule.service('$reply', ReplyService);

    const {UserService} = require('./user.service');
    ngModule.service('$user', UserService);
};