module.exports = function(ngModule){
    ngModule.constant('userStatusConstant', {
        disabled: 0,
        pending: 1,
        active: 2
    })
};