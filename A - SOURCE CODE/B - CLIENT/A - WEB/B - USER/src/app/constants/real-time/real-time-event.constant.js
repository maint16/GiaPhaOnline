module.exports = function(ngModule){
    ngModule.constant('realTimeEventConstant', {
        // Event which is raised when user registration is done.
        userRegistrationEvent: 'private-user_registered'
    })
};