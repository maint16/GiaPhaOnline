/**
 * Created by Linh Nguyen on 6/7/2017.
 */
import {AuthorizationToken} from "../../models/authorization-token";
import {ApplicationSetting} from "../../constants/application-setting";

export interface IAuthenticationService {

  //#region Properties

  //#endregion

  //#region Methods

  /*
   * Store identity into local storage.
   * */
  attachAuthorizationToken(authorizationToken: AuthorizationToken);

  /*
  * Get authorization token from local storage.
  * */
  getAuthorizationToken(): AuthorizationToken;

  /*
  * Redirect to login page.
  * */
  redirectToLogin(): void;

  //#endregion
}
