import {Observable} from "rxjs/Observable";
import {SearchUserViewModel} from "../../viewmodels/user/search-user.view-model";
import {SearchResult} from "../../models/search-result";
import {BasicLoginViewModel} from "../../viewmodels/user/basic-login.view-model";
import {User} from "../../models/entities/user";
import {AuthorizationToken} from "../../models/authorization-token";

export interface IUserService {

  //#region Methods

  /*
  * Get user personal profile
  * Pass id to get personal profile or specific person. Null for the requester's
  * */
  getPersonalProfile(id?: number): Observable<User>;

  /*
  * Use basic account to exchange for an access token.
  * */
  basicLogin(info: BasicLoginViewModel) : Observable<AuthorizationToken>;

  /*
  * Search users by using specific conditions.
  * */
  getUsers(conditions: SearchUserViewModel): Observable<SearchResult<User>>;

  //#endregion
}
