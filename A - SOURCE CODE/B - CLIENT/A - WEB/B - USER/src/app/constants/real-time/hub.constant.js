module.exports = function (ngModule) {
    ngModule.constant('hubConstant', {

        hubName: {
            notificationHub: 'hub/notification'
        },

        hubEvent:{
            receiveNotification: 'event-receive-notification'
        }
    });
};