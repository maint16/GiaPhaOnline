import {SearchAccountsViewModel} from "../../../viewmodels/accounts/search-accounts.view-model";
import {LoginViewModel} from "../../../viewmodels/accounts/login.view-model";
import {Account} from "../../../models/entities/account";
import {SubmitPasswordViewModel} from "../../../viewmodels/accounts/submit-password.view-model";
import {Response} from "@angular/http";
import {Observable} from "rxjs/Observable";

/*
 * Provides function to deal with accounts api.
 * */
export interface IAccountService {

  //#region Methods

  /*
  * Find accounts by using specific conditions.
  * */
  getAccounts(conditions: SearchAccountsViewModel): Promise<Response>;

  /*
  * Call login api to obtain client authentication token instance.
  * */
  login(loginViewModel: LoginViewModel): Promise<Response>;

  /*
  * Change account information.
  * */
  editUserProfile(index: number, information: Account): Promise<Response>;

  /*
  * Send request to service to request an instruction email to change password.
  * */
  initChangePasswordRequest(email: string): Promise<Response>;

  /*
  * Submit new password to service.
  * */
  submitPasswordReset(submitPasswordViewModel: SubmitPasswordViewModel): Promise<Response>

  /*
  * Get requester profile.
  * */
  getClientProfile(): Promise<Response>;

  //#endregion
}
