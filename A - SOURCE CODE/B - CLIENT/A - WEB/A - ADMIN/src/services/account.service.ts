import {IAccountService} from '../interfaces/services/account-service.interface';
import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {ProfileViewModel} from '../view-models/profile.view-model';
import {Observable} from 'rxjs/Observable';
// import 'rxjs/add/observable/of';
import { of } from 'rxjs';
import {LoginViewModel} from '../view-models/login.view-model';
import {ConfigUrlService} from '../constants/config-url-service.constant';

@Injectable()
export class AccountService implements IAccountService {

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
  public getProfile(): Observable<any> {
    let url = '/assets/user.json';
    let profile = new ProfileViewModel();
    profile.email = 'Email 01';
    profile.joinedTime = 0;
    profile.nickname = 'Nick name 01';
    profile.photoRelativeUrl = 'https://upload.wikimedia.org/wikipedia/commons/f/f4/User_Avatar_2.png';

    return of(profile);
  }

  public basicLogin(user: LoginViewModel): Observable<any> {
    var data = {
      email: user.email,
      password: user.password,
      captchaCode: 'abc'
    };
    return this.httpClient.post(ConfigUrlService.urlAPI + '/api/user/basic-login', data);
  }

  //#endregion

}
