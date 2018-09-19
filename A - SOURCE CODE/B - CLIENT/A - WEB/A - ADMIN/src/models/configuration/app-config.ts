import {RealTimeHubConfig} from './real-time-hub-config';
import {CloudMessagingConfig} from './cloud-messaging-config';

export class AppConfig {

  //#region Properties

  /*
  * Api end-point base url.
  * */
  public baseApiEndPoint: string = '';

  /*
  * Real-time hub configuration.
  * */
  public realTimeHub: RealTimeHubConfig;

  /*
  * Cloud messaging configuration.
  * */
  public cloudMessaging: CloudMessagingConfig;

  //#region Google authentication setting

  public googleClientId: string = '';

  //#endregion

  //#region Facebook authentication setting

  public facebookClientId: string = '';

  //#endregion

  //#endregion

}
