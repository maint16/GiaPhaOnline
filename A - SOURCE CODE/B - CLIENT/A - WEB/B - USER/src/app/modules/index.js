module.exports = (ngModule) => {

    const {AppController} = require('./app/app.controller.ts');
    ngModule.controller('appController', AppController);

    require('./shared')(ngModule);
    require('./dashboard')(ngModule);
    require('./account')(ngModule);
};