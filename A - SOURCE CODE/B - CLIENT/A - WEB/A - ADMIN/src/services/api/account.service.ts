import {Injectable, Inject} from '@angular/core';
import 'rxjs/add/operator/toPromise';
import {SearchAccountsViewModel} from "../../viewmodels/accounts/search-accounts.view-model";
import {LoginViewModel} from "../../viewmodels/accounts/login.view-model";
import {Account} from "../../models/entities/account";
import {SubmitPasswordViewModel} from "../../viewmodels/accounts/submit-password.view-model";
import {Response, ResponseOptions} from "@angular/http";
import {IAccountService} from "../../interfaces/services/api/account-service.interface";
import {IApiService} from "../../interfaces/services/api/api-service.interface";
import {environment} from "../../environments/environment";
import {ApiUrl} from "../../constants/api-url";

/*
 * Service which handles category business.
 * */
@Injectable()
export class AccountService implements IAccountService {

  //#region Constructor

  /*
  * Initiate instance of category service.
  * */
  public constructor(@Inject("IApiService") public apiService: IApiService) {
  }

  //#endregion

  //#region Methods

  /*
  * Find categories by using specific conditions.
  * */
  public getAccounts(conditions: SearchAccountsViewModel): Promise<Response> {
    // Request to api to obtain list of available categories in system.
    return this.apiService.post(environment.baseUrl, ApiUrl.getAccounts,
      null,
      conditions).toPromise();
  }

  /*
  * Sign an account into system.
  * */
  public login(loginViewModel: LoginViewModel): Promise<Response> {
    return this.apiService
      .post(environment.baseUrl, ApiUrl.login, null, loginViewModel)
      .toPromise();
  }

  /*
  * Change account information in service.
  * */
  public editUserProfile(index: number, information: Account): Promise<Response> {

    // Build a complete url of account information change.
    let urlParameters = {
      id: index
    };

    return this.apiService.put(environment.baseUrl, ApiUrl.editAccount, urlParameters, information)
      .toPromise();
  }

  /*
  * Request service to send an email which is for changing account password.
  * */
  public initChangePasswordRequest(email: string): Promise<Response> {
    // Parameter construction.
    let urlParameters = {
      email: email
    };

    return this.apiService.get(environment.baseUrl, ApiUrl.requestPasswordReset, urlParameters)
      .toPromise();
  }

  /*
  * Request service to change password by using specific token.
  * */
  public submitPasswordReset(submitPasswordViewModel: SubmitPasswordViewModel): Promise<Response> {
    return this.apiService
      .post(environment.baseUrl, ApiUrl.initPasswordSubmission,
        null,
        submitPasswordViewModel)
      .toPromise();
  }

  /*
  * Request service to return account profile.
  * */
  public getClientProfile(id): Promise<Response> {

    if (id == null)
      id = 0;

    let szUrl = `${environment.baseUrl}/${ApiUrl.getAccountProfile}`;
    szUrl = szUrl.replace('{id}', id);

    return this.apiService
      .get(
        environment.baseUrl, ApiUrl.getAccountProfile.replace('{id}', id),
        null).toPromise();
  }


  //#endregion
}
