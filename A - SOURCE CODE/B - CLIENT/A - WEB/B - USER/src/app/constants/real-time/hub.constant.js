module.exports = function (ngModule) {
    ngModule.constant('hubConstant', {

        hubName: {
            notificationHub: 'notification-hub'
        },

        hubEvent:{
            receiveNotification: 'event-receive-notification'
        }
    });
};