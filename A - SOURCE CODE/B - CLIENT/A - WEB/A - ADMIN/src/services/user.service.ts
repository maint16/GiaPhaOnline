import {IUserService} from "../interfaces/services/user-service.interface";
import {User} from "../models/entities/user";
import {BasicLoginViewModel} from "../viewmodels/user/basic-login.view-model";
import {Observable} from "rxjs/Observable";
import {SearchResult} from "../models/search-result";
import {SearchUserViewModel} from "../viewmodels/user/search-user.view-model";
import {HttpClient} from "@angular/common/http";
import {environment} from "../environments/environment";
import {UserApiUrlConstant} from "../constants/api-urls/user-api-url.constant";
import 'rxjs/add/operator/map';
import {AuthorizationToken} from "../models/authorization-token";
import {Injectable} from "@angular/core";

@Injectable()
export class UserService implements IUserService{


  //#region Constructor

  /*
  * Initialize service with injector.
  * */
  public constructor(private httpClient: HttpClient){}

  //#endregion

  //#region Methods

  /*
  * Get user personal profile
  * Pass id to get personal profile or specific person. Null for the requester's
  * */
  public getPersonalProfile(id?: number): Observable<User> {
    let fullUrl = `${environment.baseUrl}/${UserApiUrlConstant.getPersonalProfile}/${id == null ? 0: id}`;
    return this.httpClient.get<User>(fullUrl);
  }

  /*
  * Exchanging user information for an access token.
  * */
  public basicLogin(info: BasicLoginViewModel): Observable<AuthorizationToken> {

    // Construct full url to make request to.
    let fullUrl = `${environment.baseUrl}/${UserApiUrlConstant.basicLogin}`;

    // Manage the request.
    return this.httpClient.post<AuthorizationToken>(fullUrl, info);
  }

  /*
  * Using search conditions to get a list of users.
  * */
  public getUsers(conditions: SearchUserViewModel): Observable<SearchResult<User>> {
    // Construct full url to make request to.
    let fullUrl = `${environment.baseUrl}/${UserApiUrlConstant.getUsers}`;

    // Manage the request.
    return this.httpClient.post<SearchResult<User>>(fullUrl, conditions);
  }

  //#endregion
}
