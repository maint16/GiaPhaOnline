module.exports = (ngModule) => {
    require('./shared')(ngModule);
    require('./dashboard')(ngModule);
    require('./account')(ngModule);
};