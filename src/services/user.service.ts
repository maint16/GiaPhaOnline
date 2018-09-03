import {IUserService} from '../interfaces/services/user-service.interface';
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import 'rxjs/add/observable/of';
import {ConfigUrlService} from '../constants/config-url-service.constant';
@Injectable()
export class UserService implements IUserService {

  //#region Constructor

  /*
  * Initiate service with injectors.
  * */
  public constructor(public httpClient: HttpClient) {

  }

  //#endregion

  //#region Methods

  /*
  * Get profile information.
  * */
  public getUser() {
    return this.httpClient.get(ConfigUrlService.urlAPI + '/api/user/search');
  }
  //#endregion

}
