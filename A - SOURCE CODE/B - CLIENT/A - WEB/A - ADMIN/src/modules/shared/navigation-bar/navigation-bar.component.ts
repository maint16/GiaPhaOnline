import {Component, Inject, Input} from '@angular/core';
import {Router} from '@angular/router';
import {IAuthenticationService} from '../../../interfaces/services/authentication-service.interface';
import {ProfileViewModel} from '../../../view-models/profile.view-model';
import {LocalStorageService} from 'ngx-localstorage';
import {LocalStorageKeyConstant} from '../../../constants/local-storage-key.constant';
import * as firebase from 'firebase';
import {SignOutViewModel} from '../../../view-models/user/sign-out.view-model';
import {IUserService} from '../../../interfaces/services/user-service.interface';
import {TranslateService} from '@ngx-translate/core';
import {ToastrService} from 'ngx-toastr';
import {Observable} from 'rxjs/Rx';
import {flatMap} from 'tslint/lib/utils';

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
  public constructor(@Inject('IAuthenticationService') public authenticationService: IAuthenticationService,
                     @Inject('IUserService') public userService: IUserService,
                     public translateService: TranslateService,
                     public localStorageService: LocalStorageService,
                     public toastr: ToastrService,
                     public router: Router) {
  }

  //#endregion

  //#region Methods

  /*
  * Sign the user out.
  * */
  public clickSignOut(): void {
    // TODO: Clear user cloud messaging device token.
    let model = new SignOutViewModel();

    // TODO: Disconnect user real-time channel.

    let messaging = firebase.messaging();

    // Get device token to unregister it from notification system.
    let pGetUserDeviceTokenIdPromise = messaging
      .getToken();

    Observable
      .fromPromise(pGetUserDeviceTokenIdPromise)
      .flatMap((deviceTokenId: string) => {
        model.deviceIds = [deviceTokenId];
        return this.userService
          .signOut(model);
      })
      .subscribe(() => {
        // Remove token from local storage.
        this.localStorageService.remove(LocalStorageKeyConstant.accessToken);
        let message = this.translateService.instant('MSG_SIGN_OUT_SUCCESSFULLY');
        this.toastr.success(message);
        // Re-direct to login page.
        this.router.navigate(['/login']);
      });
  }

  //#endregion
}
