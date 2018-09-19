import {Component, Inject, Input} from '@angular/core';
import {Router} from "@angular/router";
import {IAuthenticationService} from "../../../interfaces/services/authentication-service.interface";
import {ProfileViewModel} from "../../../view-models/profile.view-model";
import {LocalStorageService} from 'ngx-localstorage';
import {LocalStorageKeyConstant} from '../../../constants/local-storage-key.constant';
import * as firebase from 'firebase';

@Component({
  selector: 'navigation-bar',
  templateUrl: 'navigation-bar.component.html'
})

export class NavigationBarComponent {

  //#region Properties

  // Account property.
  @Input('profile')
  private profile: ProfileViewModel;

  //#endregion

  //#region Constructor

  // Initiate instance with IoC.
  public constructor(@Inject("IAuthenticationService") public authenticationService: IAuthenticationService,
                     public localStorageService: LocalStorageService,
                     public router: Router) {
  }

  //#endregion

  //#region Methods

  /*
  * Sign the user out.
  * */
  public clickSignOut(): void {
    // TODO: Clear user cloud messaging device token.

    // TODO: Disconnect user real-time channel.

    let messaging = firebase.messaging();
    messaging
      .getToken()
      .then((token: string) => {
        console.log(`Id token = ${token}`);
      });

    // Remove token from local storage.
    this.localStorageService.remove(LocalStorageKeyConstant.accessToken);

    // Re-direct to login page.
    this.router.navigate(['/login']);
  }

  //#endregion
}
