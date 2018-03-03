module.exports = function(ngModule){
    ngModule.constant('taskStatusConstant', {
        beforeAction: 0,
        actioning: 1,
        afterAction: 2
    });
};