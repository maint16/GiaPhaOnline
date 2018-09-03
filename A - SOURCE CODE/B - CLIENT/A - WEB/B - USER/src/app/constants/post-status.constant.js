module.exports = function(ngModule){
    ngModule.constant('postStatusConstant', {
        disabled: 0,
        available: 1,
        deleted: 2
    });
};