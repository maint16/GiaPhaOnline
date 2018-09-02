module.exports = (ngModule) => {

    const {UiService} = require('./ui.service');
    ngModule.service('$ui', UiService);

    const {CategoryGroupService} = require('./category-group.service');
    ngModule.service('$categoryGroup', CategoryGroupService);

    const {CategoryService} = require('./category.service');
    ngModule.service('$category', CategoryService);

    const {TopicService} = require('./topic.service');
    ngModule.service('$topic', TopicService);

    const {ReplyService} = require('./reply.service');
    ngModule.service('$reply', ReplyService);

    const {UserService} = require('./user.service');
    ngModule.service('$user', UserService);

    const {FileService} = require('./file.service');
    ngModule.service('$file', FileService);

    const {AuthService} = require('./auth.service');
    ngModule.service('$auth', AuthService);
};