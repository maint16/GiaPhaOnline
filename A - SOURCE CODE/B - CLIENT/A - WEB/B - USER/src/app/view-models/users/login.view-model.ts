export class LoginViewModel {

    //#region Properties

    /*
    * Email which has been used for registering account.
    * */
    public email: string = '';

    /*
    * Password which is attached to email.
    * */
    public password: string = '';

    /*
    * Captcha code which is provided by 3rd party provider.
    * */
    public captchaCode: string = '';

    //#endregion

}