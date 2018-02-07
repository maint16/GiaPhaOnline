module.exports = function(ngModule){
    ngModule.constant('postStatus', {
        disabled: 0,
        available: 1,
        deleted: 2
    });
};