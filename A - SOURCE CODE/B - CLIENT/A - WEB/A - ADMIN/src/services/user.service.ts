import {IUserService} from '../interfaces/services/user-service.interface';
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import 'rxjs/add/observable/of';
import {ConfigUrlService} from '../constants/config-url-service.constant';
import {ConfigUrlUserServiceConstant} from '../constants/config-url-user-service.constant';

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
  public searchUser()
    {
      var data = {};
      var url = ConfigUrlService.urlAPI + '/' + ConfigUrlUserServiceConstant.searchUser;
      return this.httpClient.post(url, data);
    }
  public getUserDetail(id)
    {
      var url = ConfigUrlService.urlAPI + '/' + ConfigUrlUserServiceConstant.getUserDetail;
      url.replace('{id}', id);
      return this.httpClient.get(url);
    }
  public saveUserStatus(id, status)
    {
      var url = ConfigUrlService.urlAPI + '/' + ConfigUrlUserServiceConstant.saveUserStatus;
      url.replace('{id}', id);
      var body = {
        status: status
      };
      return this.httpClient.put(url, body);
    }
    //#endregion

  }
