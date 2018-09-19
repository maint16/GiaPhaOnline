export class CloudMessagingConfig {

  //#region Properties

  /*
  * API key which can be found at : https://console.firebase.google.com/project/<project>/settings/cloudmessaging/
  * */
  public webApiKey: string = '';

  /*
  * Relative path of service worker file to use for handling cloud messages.
  * */
  public serviceWorkerPath: string = '';

  /*
  * Sender ID for receiving cloud messages. This must match to project's.
  * Can be found at: https://console.firebase.google.com/project/<project>/settings/cloudmessaging/
  * */
  public messagingSenderId: string = '';

  //#endregion
}
