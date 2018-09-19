import {Component, Inject, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ProfileViewModel} from '../../../view-models/profile.view-model';
import * as signalR from '@aspnet/signalr';
import {RealTimeEventConstant} from '../../../constants/real-time/real-time-event.constant';
import {LocalStorageService} from 'ngx-localstorage';
import {LocalStorageKeyConstant} from '../../../constants/local-storage-key.constant';
import {ISharedEventService} from '../../../interfaces/services/shared-event-service.interface';
import {RealTimeChannelConstant} from '../../../constants/real-time/real-time-channel.constant';
import * as firebase from 'firebase';
import {AppConfig} from '../../../models/configuration/app-config';
import {AppConfigService} from '../../../services/app-config.service';

@Component({
  selector: 'authorize-layout',
  templateUrl: 'authorize-layout.component.html'
})

export class AuthorizeLayoutComponent implements OnInit {

  //#region Properties

  /*
  * Profile information.
  * */
  private profile: ProfileViewModel;

  /*
  * App configuration.
  * */
  private appConfig: AppConfig;

  //#endregion

  //#region Constructor

  /*
  * Initiate component with injectors.
  * */
  public constructor(public activatedRoute: ActivatedRoute,
                     public localStorageService: LocalStorageService,
                     @Inject('ISharedEventService') public sharedEventService: ISharedEventService,
                     public appConfigService: AppConfigService) {

    this.appConfig = this.appConfigService.loadAppConfig();
    if (!this.appConfig)
      throw 'No app configuration has been loaded.';
  }

  //#endregion

  //#region Methods

  /*
  * Event which is called when component has been initiated.
  * */
  public ngOnInit(): void {
    this.activatedRoute
      .data
      .subscribe((x: any) => {
        this.profile = <ProfileViewModel> x.profile;

        // Register real-time communication handlers.
        this.ngRegisterRealTimeCommunicationHandlers();

        // Register cloud messaging service workers.
        this.ngRegisterCloudMessagingServiceWorker();
      });
  }

  /*
  * Register real-time
  * */
  private ngRegisterRealTimeCommunicationHandlers(): void {
    let connection = new signalR
      .HubConnectionBuilder()
      .withUrl('http://localhost:61356/hub/notification', {
        accessTokenFactory: () => {
          return this.localStorageService.get(LocalStorageKeyConstant.accessToken);
        }
      })
      .build();

    connection
      .on(RealTimeEventConstant.addCategoryGroup, data => {
        this.sharedEventService
          .addMessage(RealTimeChannelConstant.notification, RealTimeEventConstant.addCategoryGroup, data);
      });

    connection.start()
      .then(() => {
        console.log('Real-time connection has been established.');
      });
  }

  /*
  * Register
  * */
  private ngRegisterCloudMessagingServiceWorker(): void {

    // Get cloud messaging configuration.
    let cloudMessaging = this.appConfig.cloudMessaging;
    if (!cloudMessaging || !cloudMessaging.messagingSenderId || !cloudMessaging.webApiKey) {
      console.log('Cloud messaging configuration is invalid. Skipping cloud messaging configuration...');
      return;
    }

    // Initialize firebase app.
    firebase
      .initializeApp({
        messagingSenderId: cloudMessaging.messagingSenderId
      });

    // Get messaging instance.
    const messaging = firebase.messaging();
    messaging.usePublicVapidKey(cloudMessaging.webApiKey);

    let firebaseServiceWorkerPath = 'firebase-messaging-sw.js';
    if (cloudMessaging.serviceWorkerPath)
      firebaseServiceWorkerPath = cloudMessaging.serviceWorkerPath;

    // Add additional data to service worker.
    firebaseServiceWorkerPath += `?messagingSenderId=${cloudMessaging.messagingSenderId}`;

    let pAddServiceWorkerTask = new Promise(resolve => {
      navigator
        .serviceWorker
        .register(firebaseServiceWorkerPath)
        .then((serviceWorkerRegistration: ServiceWorkerRegistration) => {
          messaging.useServiceWorker(serviceWorkerRegistration);
          resolve();
        });
    });

    // Called when firebase cloud messaging attached to a specific service worker.
    pAddServiceWorkerTask
      .then(() => {
        messaging
          .requestPermission()
          .then(() => {
            messaging
              .getToken()
              .then((cloudMessagingToken: string) => {
                if (!cloudMessagingToken) {
                  // Show permission request.
                  console.log('No Instance ID token available. Request permission to generate one.');
                  return;
                }
                console.log(cloudMessagingToken);
              }).catch((exception) => {
              console.log('An error occurred while retrieving token. ', exception);
            });
          })
          .catch((exception) => {
            console.log('Unable to get permission to notify.', exception);
          });
      });
  }

  //#endregion
}
