import {ProfileViewModel} from "../../view-models/profile.view-model";
import {Observable} from "rxjs/Observable";
import {TokenViewModel} from '../../view-models/token.view-model';

export interface IAccountService {

  //#region Methods

  /*
  * Get profile information.
  * */
  getProfile(): Observable<ProfileViewModel>;

  //#endregion

}
