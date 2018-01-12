import {AccountStatus} from "../../enumerations/account-status";

/*
* Account and its properties.
* */
export class Account {

  //#region Properties

  /*
  * Account index.
  * */
  public id: number;

  /*
  * Email which is used for account registration.
  * */
  public email: string;

  /*
  * Nickname of account.
  * */
  public nickname: string;

  /*
  * Status of account.
  * */
  public status: AccountStatus;

  /*
  * Photo of account.
  * */
  public photoRelativeUrl: string;

  /*
  * Time when account was created on server.
  * */
  public joined: number;

  /*
  * Time when account was lastly modified.
  * */
  public lastModified: number;

  //#endregion
}
