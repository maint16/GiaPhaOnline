import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {AppConfig} from '../models/app-config';

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
  public loadConfigurationFromFile(): Promise<AppConfig> {
    return this.httpClient
      .get('/assets//app.config.json')
      .toPromise()
      .then(data => {
        console.log(data);
        let options = <AppConfig> data;
        this._appConfiguration = options;
        return options;
      });
  }

  /*
  * Load configuration from cache.
  * */
  public loadConfigurationFromCache(): AppConfig {
    return this._appConfiguration;
  }

  //#endregion
}
