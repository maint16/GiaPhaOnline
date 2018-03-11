import {Response} from '@angular/http';
import {KeyValuePair} from "../../models/key-value-pair";

export interface IConfigurationService {

  //#region Methods

  /*
  Get list of account statuses (key-value)
  */
  getAccountStatuses(): Promise<Response>;

  /*
  * Get sort directions configured in setting file.
  * */
  getSortDirections(): Promise<Response>;

  /*
 * Get key name from value.
 * */
  getKeyByValue(keyValuePairs: Array<KeyValuePair<any>>, value: any): string;

  //#endregion
}
