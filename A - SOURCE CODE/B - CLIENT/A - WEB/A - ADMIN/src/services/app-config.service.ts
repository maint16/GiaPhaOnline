import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {AppConfig} from '../models/configuration/app-config';

@Injectable()
export class AppConfigService {

  //#region Properties

  private _appConfiguration: AppConfig;

  //#endregion

  //#region Constructors

  constructor(public httpClient: HttpClient) {

  }

  //#endregion

  //#region Application configuration

  /*
  * Load app configuration from json file.
  * */
  public loadAppConfigAsync(): Promise<AppConfig> {
    return this.httpClient
      .get('/src/assets/app.config.json')
      .toPromise()
      .then(data => {
        let options = <AppConfig> data;
        this._appConfiguration = options;
        return options;
      });
  }

  /*
  * Load app config from cache.
  * */
  public loadAppConfig(): AppConfig {
    return this._appConfiguration;
  }

  //#endregion
}
