/**
 * Here is is the code snippet to initialize Firebase Messaging in the Service
 * Worker when your app is not hosted on Firebase Hosting.
 // [START initialize_firebase_in_sw]
 // Give the service worker access to Firebase Messaging.
 // Note that you can only use Firebase Messaging here, other Firebase libraries
 // are not available in the service worker.

 // [END initialize_firebase_in_sw]
 **/

importScripts('https://www.gstatic.com/firebasejs/4.8.1/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/4.8.1/firebase-messaging.js');

//#eregion Methods
self.addEventListener('activate', event => {
  addCloudMessagingBackgroundWorker();
  console.log('Service worker is being activated');
});

addCloudMessagingBackgroundWorker = () => {
  const url = new URL(self.location);
  if (!url)
    throw 'Url is invalid';

  let urlSearchParams = url.searchParams;
  if (!urlSearchParams)
    throw 'Param is not defined';

  const messagingSenderId = urlSearchParams.get('messagingSenderId');
  if (!messagingSenderId) {
    console.log('No messagingSenderId is passed to service worker.');
    return;
  }

  firebase.initializeApp({
    'messagingSenderId': messagingSenderId
  });

  const messaging = firebase.messaging();
  messaging
    .setBackgroundMessageHandler((payload) => handleCloudMessagingPayload(payload));
  console.log('Cloud messaging background handler has been initialized');
};

/*
* Handle cloud messaging payload.
* */
handleCloudMessagingPayload = (payload) => {
  // Customize notification here
  let notificationTitle = 'Background Message Title';
  let notificationOptions = {
    body: 'Background Message body.',
    icon: '/firebase-logo.png'
  };

  return self.registration
    .showNotification(notificationTitle,
      notificationOptions);
};

//#endregion

// Initialize cloud messaging background worker.
addCloudMessagingBackgroundWorker();


