import {IFormController, IScope} from "angular";
import {LoginViewModel} from "../../../view-models/users/login.view-model";
import {AppSetting} from "../../../models/app-setting";

export interface ILoginScope extends IScope {

    //#region Properties

    /*
    * Login model which is for information binding.
    * */
    loginModel: LoginViewModel;

    /*
    * Login form instance.
    * */
    loginForm: IFormController;

    //#endregion

    //#region Methods

    /*
    * Called when login button is clicked.
    * */
    ngOnLoginClicked(): void;

    /*
    * Called when forgot password is clicked.
    * */
    ngOnForgotPasswordClicked(): void;

    /*
    * Called when basic-register button is clicked.
    * */
    ngOnRegisterClicked(): void;

    // Called when google login is clicked.
    ngOnGoogleLoginClicked(): void;

    // Check whether user can do Google login.
    ngIsAbleToLoginGoogle(): void;

    //#endregion

}