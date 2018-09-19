import {RealTimeHubConfig} from './real-time-hub-config';
import {CloudMessagingConfig} from './cloud-messaging-config';

export class AppConfig {

  //#region Properties

  /*
  * Api end-point base url.
  * */
  public baseUrl: string = '';

  /*
  * Real-time hub configuration.
  * */
  public realTimeHub: RealTimeHubConfig;

  /*
  * Cloud messaging configuration.
  * */
  public cloudMessaging: CloudMessagingConfig;

  //#endregion

}
