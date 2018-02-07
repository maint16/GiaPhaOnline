module.exports = function(ngModule){
    require('./shared')(ngModule);
    require('./dashboard')(ngModule);
    require('./account')(ngModule);
    require('./category')(ngModule);
};