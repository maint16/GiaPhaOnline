import {Observable} from 'rxjs/Observable';
import {LoadUserViewModel} from '../../view-models/user/load-user.view-model';
import {User} from '../../models/entities/user';
import {SearchResult} from '../../models/search-result';
import {EditUserStatusViewModel} from '../../view-models/user/edit-user-status.view-model';
export interface IUserService {
  //#region Methods
  /*
 * Get profile information.
 * */
  getUser();
  // Load users from api end-point.
  loadUsers(conditions: LoadUserViewModel): Observable<SearchResult<User>>;


  getUserDetail(id: number);

  // Edit user status using specific condition.
  editUserStatus(conditions: EditUserStatusViewModel): Observable<any>;

  //#endregion
}
