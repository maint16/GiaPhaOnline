module.exports = function(ngModule){
  ngModule.constant('realTimeChannelConstant', {

    // Realtime channels belong to admin.
    admin:{
        userRegistration: 'private-user_registered'
    }
  });
};