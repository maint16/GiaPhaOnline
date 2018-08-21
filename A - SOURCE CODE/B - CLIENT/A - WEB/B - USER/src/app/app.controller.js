module.exports = (ngModule) => {
    /*
* Application controller.
* */
    ngModule.controller('appController', ($transitions, $timeout,
                                          uiService, pushNotificationService,
                                          $scope) => {

        //#region Properties

        // For two-way model binding.
        $scope.model = {
            layoutClass: ''
        };

        // Check whether fcm service has been initialized before.
        $scope.bIsFcmInitialized = false;

        //#endregion

        //#region Methods

        //#endregion

        //#region Watchers & events

        /*
        * Called when transition from state to state is successful.
        * */
        $transitions.onSuccess({}, ($transition) => {

            $timeout(() => {

                // Reload window size.
                uiService.reloadWindowSize();

                if (!$scope.bIsFcmInitialized) {
                    // Initialize firebase.
                    firebase.initializeApp({
                        'messagingSenderId': '420319602777'
                    });

                    // Request for push notification service.
                    let messaging = firebase.messaging();
                    messaging.usePublicVapidKey("BKa5DHBtGv4JchD7XuP571kHQKM-7T-5Bdj5KM3flRjVFDVtTDtX6CEe_WEeHuwmoV0O1DaQ7KClP6kqG618--A");
                    messaging.requestPermission()
                        .then(() => {
                            return messaging.getToken()
                                .then((fcmToken) => {
                                    return fcmToken;
                                })
                                .catch((authenticationError) => {
                                    console.log('An error occurred while retrieving token. ', authenticationError);
                                    throw 'An error occurred while retrieving token.';
                                });
                        })
                        .then((fcmToken) => {

                            // Initialize add device condition.
                            let addDeviceCondition = {
                                deviceId: fcmToken
                            };

                            // Mark fcm service as has been initialized.
                            $scope.bIsFcmInitialized = true;

                            // Call api service to add device.
                            return pushNotificationService.addDevice(addDeviceCondition);
                        })
                        .catch(() => {
                            console.log('Unable to get permission to notify.');
                        });
                }


            }, 250);

            // Find destination of transaction.
            let destination = $transition.$to();

            if (destination.includes[UrlStateConstant.authorizedLayoutModuleName]) {
                $scope.model.layoutClass = 'hold-transition skin-green-light layout-top-nav';
                return;
            }

            let urlStateUser = urlStates.user;
            if (destination.includes[UrlStateConstant.loginModuleName] || destination.includes[UrlStateConstant.googleLoginModuleName]) {
                $scope.model.layoutClass = 'hold-transition login-page';
                return;
            }

            $scope.model.layoutClass = 'hold-transition';
        });

        //#endregion
    });
};