import {Observable} from 'rxjs/Observable';
import {LoadUserViewModel} from '../../view-models/user/load-user.view-model';
import {User} from '../../models/entities/user';
import {SearchResult} from '../../models/search-result';
import {EditUserStatusViewModel} from '../../view-models/user/edit-user-status.view-model';
import {TokenViewModel} from '../../view-models/token.view-model';
import {GoogleLoginViewModel} from '../../view-models/user/google-login.view-model';
import {SignOutViewModel} from '../../view-models/user/sign-out.view-model';

export interface IUserService {

  //#region Methods

  /*
   * Get profile information.
   * */
  getUser();

  // Load users from api end-point.
  loadUsers(conditions: LoadUserViewModel): Observable<SearchResult<User>>;

  /*
  * Get user profile.
  * */
  getUserDetail(id: number);

  // Edit user status using specific condition.
  editUserStatus(conditions: EditUserStatusViewModel): Observable<any>;

  // Exchange code returned by Google with system access token.
  googleLogin(model: GoogleLoginViewModel): Observable<TokenViewModel>;

  // Sign user out from system.
  signOut(model: SignOutViewModel): Observable<any>;

  //#endregion
}
