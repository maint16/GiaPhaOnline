/**
 * Created by Linh Nguyen on 6/7/2017.
 */
import {Inject, Injectable} from "@angular/core";
import {CanActivate, Router} from "@angular/router";
import {IAuthenticationService} from "../interfaces/services/authentication-service.interface";
import {LocalStorageService} from 'ngx-localstorage';
import {LocalStorageKeyConstant} from '../constants/local-storage-key.constant';

@Injectable()
export class IsAuthorizedGuard implements CanActivate {

  //#region Constructor
  /*
  * Initiate guard component with injectors.
  * */
  public constructor(
    @Inject('IAuthenticationService') private authenticationService: IAuthenticationService,
    public localStorageService: LocalStorageService,
    private router: Router) {}

  //#endregion

  //#region Methods

  /*
  * Check whether route can be activated or not.
  * */
  public canActivate(): boolean {

    // Get access token.
    let accessToken: string = this.localStorageService.get(LocalStorageKeyConstant.accessToken);

    // No access token has been stored.
    if (!accessToken) {
      // Redirect to login.
      this.router.navigate(['/login']);
      return false;
    }

    return true;
  }

  //#endregion
}
