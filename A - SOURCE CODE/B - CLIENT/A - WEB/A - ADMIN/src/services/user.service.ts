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
import {EditUserStatusViewModel} from '../view-models/user/edit-user-status.view-model';
import {AppConfigService} from './app-config.service';
import {ProfileViewModel} from '../view-models/profile.view-model';
import {GoogleLoginViewModel} from '../view-models/user/google-login.view-model';
import {TokenViewModel} from '../view-models/token.view-model';
import {SignOutViewModel} from '../view-models/user/sign-out.view-model';

@Injectable()
export class UserService implements IUserService {

  //#region Properties

  // Base api end-point.
  private baseApiEndPoint: string = '';

  //#endregion

  //#region Constructor

  /*
  * Initiate service with injectors.
  * */
  public constructor(public httpClient: HttpClient,
                     public appConfigService: AppConfigService) {
    let appConfig = this.appConfigService.loadAppConfig();
    this.baseApiEndPoint = appConfig.baseApiEndPoint;
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
    const url = `${this.baseApiEndPoint}/${ConfigUrlUserServiceConstant.searchUser}`;
    return this.httpClient
      .post<SearchResult<User>>(url, condition);
  }

  /*
  * Get user information by using specific information.
  * */
  public getUserDetail(id) {
    let url = `${this.baseApiEndPoint}/${ConfigUrlUserServiceConstant.getUserDetail}`;
    url = url.replace('{id}', id);
    return this.httpClient.get(url);
  }


  /*
  * Edit user status using specific information.
  * */
  public editUserStatus(condition: EditUserStatusViewModel): Observable<any> {
    let url = `${this.baseApiEndPoint}/${ConfigUrlUserServiceConstant.editUserStatus}`;
    url = url.replace('{id}', `${condition.userId}`);

    return this.httpClient
      .put(url, condition);
  }

  // Add user device to push notification.
  public addUserDevice(deviceId: string): Observable<any> {
    let url = `${this.baseApiEndPoint}/api/real-time/subscribe-push-device`;
    let model = {
      deviceId: deviceId
    };

    return this.httpClient
      .post(url, model);
  }

  /*
  * Exchange google code with system access token.
  * */
  public googleLogin(model: GoogleLoginViewModel): Observable<TokenViewModel> {
    let url = `${this.baseApiEndPoint}/api/user/google-login`;
    return this.httpClient
      .post<TokenViewModel>(url, model);
  }

  /*
  * Sign user out from system.
  * */
  public signOut(model: SignOutViewModel): Observable<any> {
    let url = `${this.baseApiEndPoint}/api/user/sign-out`;
    return this.httpClient
      .post<any>(url, model);
  }


  //#endregion

}
