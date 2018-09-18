import {Component, Inject, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ProfileViewModel} from '../../../view-models/profile.view-model';
import * as signalR from '@aspnet/signalr';
import {RealTimeEventConstant} from '../../../constants/real-time/real-time-event.constant';
import {LocalStorageService} from 'ngx-localstorage';
import {LocalStorageKeyConstant} from '../../../constants/local-storage-key.constant';
import {ISharedEventService} from '../../../interfaces/services/shared-event-service.interface';
import {RealTimeChannelConstant} from '../../../constants/real-time/real-time-channel.constant';

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

  //#endregion

  //#region Constructor

  /*
  * Initiate component with injectors.
  * */
  public constructor(public activatedRoute: ActivatedRoute,
                     public localStorageService: LocalStorageService,
                     @Inject('ISharedEventService') public sharedEventService: ISharedEventService) {
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


        // Add signalr connection.
        let connection = new signalR.HubConnectionBuilder()
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

      });
  }

  //#endregion
}
