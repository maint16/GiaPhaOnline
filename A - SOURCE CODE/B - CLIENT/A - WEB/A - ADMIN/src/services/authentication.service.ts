/**
 * Created by Linh Nguyen on 6/7/2017.
 */
import {Injectable} from "@angular/core";
import {AuthorizationToken} from "../models/authorization-token";
import {Router} from "@angular/router";
import {IAuthenticationService} from "../interfaces/services/authentication-service.interface";
import {ApplicationSetting} from "../constants/application-setting";

@Injectable()
export class AuthenticationService implements IAuthenticationService {

  //#region Constructor

  /*
  * Initiate component with injectors.
  * */
  public constructor(private router: Router){

  }

  //#endregion

  //#region Methods

  /*
   * Store identity into local storage.
   * */
  public attachAuthorizationToken(authorizationToken: AuthorizationToken): void {
    localStorage.setItem(ApplicationSetting.identityStorage, JSON.stringify(authorizationToken));
  }

  /*
  * Get authorization token from local storage.
  * */
  public getAuthorizationToken(): AuthorizationToken{

    // Get authorization token from local storage.
    let szAuthorizationToken = localStorage.getItem(ApplicationSetting.identityStorage);

    // Authorization is invalid.
    if (szAuthorizationToken == null || szAuthorizationToken.length < 1)
      return null;

    // Parse authorization token.
    let authorizationToken = <AuthorizationToken> JSON.parse(szAuthorizationToken);

    if (!this.bIsAuthorizationValid(authorizationToken))
      return null;

    return authorizationToken;
  };

  /*
  * Check whether authorization token is valid or not.
  * */
  private bIsAuthorizationValid(authorizationToken: AuthorizationToken): boolean{

    // Token is not valid.
    if (authorizationToken == null)
      return false;

    // Authorization token code is not valid.
    if (authorizationToken.code == null || authorizationToken.code.length < 1)
      return false;

    // // Authorization token has been expired.
    // if (authorizationToken.expire >= Date.now())
    //   return false;

    return true;
  };

  /*
  * Redirect to login page.
  * */
  public redirectToLogin(): void{
    this.router.navigate(['/login']);
  }

  //#endregion
}
