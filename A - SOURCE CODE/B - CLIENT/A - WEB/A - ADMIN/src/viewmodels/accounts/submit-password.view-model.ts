export class SubmitPasswordViewModel {

  //#region Properties

  /*
  * Address of account.
  * */
  public email: string;

  /*
  * Token which is used for changing account password.
  * */
  public token: string;

  /*
  * New password of account.
  * */
  public password: string;

  /*
  * Password confirmation.
  * */
  public passwordConfirmation: string;

  //#endregion
}
