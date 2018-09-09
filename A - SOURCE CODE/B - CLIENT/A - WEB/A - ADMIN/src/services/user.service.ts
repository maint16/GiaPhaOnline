import {IUserService} from '../interfaces/services/user-service.interface';
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import 'rxjs/add/observable/of';
import {ConfigUrlService} from '../constants/config-url-service.constant';
import {ConfigUrlUserServiceConstant} from '../constants/config-url-user-service.constant';
import {LoadUserViewModel} from '../view-models/user/load-user.view-model';
import {Observable} from 'rxjs/Rx';
import {SearchResult} from '../models/search-result';
import {User} from '../models/entities/user';
import {SaveUserStatusViewModel} from '../view-models/user/save-user-status.view-model';

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

  // Load users by using specific conditions.
  public loadUsers(condition: LoadUserViewModel): Observable<SearchResult<User>> {
      const url = `${ConfigUrlService.urlAPI}/${ConfigUrlUserServiceConstant.searchUser}`;
      return this.httpClient
        .post<SearchResult<User>>(url, condition);
    }
  public getUserDetail(id)
    {
      const url = ConfigUrlService.urlAPI + '/' + ConfigUrlUserServiceConstant.getUserDetail;
      url = url.replace('{id}', id);
      return this.httpClient.get(url);
    }
  public saveUserStatus(condition: SaveUserStatusViewModel)
    {
      const url = ConfigUrlService.urlAPI + '/' + ConfigUrlUserServiceConstant.saveUserStatus;
      url.replace('{id}', String(condition.userId));
      let body = {
        status: condition.userStatus
      };
      return this.httpClient.put(url, body);
    }
    //#endregion

  }
