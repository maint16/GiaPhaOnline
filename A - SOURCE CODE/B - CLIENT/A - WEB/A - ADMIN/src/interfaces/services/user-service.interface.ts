import {UserViewModel} from '../../view-models/user.view-model';
import {Observable} from 'rxjs/Observable';

export interface IUserService {

  //#region Methods

  /*
  * Get profile information.
  * */
  getUser();
  searchUser(data: any);
  getUserDetail(id: any);
  saveUserStatus(id: any, status: number);
  //#endregion

}
