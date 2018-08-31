import {IAngularEvent, IFormController, IScope} from "angular";
import {ForgotPasswordViewModel} from "../../../view-models/users/forgot-password.view-model";
import {IRecaptchaScope} from "../../../interfaces/scopes/recaptcha.scope";

export interface IForgotPasswordScope extends IScope, IRecaptchaScope {

    //#region Properties

    // Model for information binding.
    forgotPasswordModel: ForgotPasswordViewModel;

    // Forgot password form instance.
    forgotPasswordForm: IFormController;

    //#endregion

    //#region Methods

    /*
    * Called when login is clicked.
    * */
    ngOnLoginClicked(): void;

    /*
    * Called when account basic-register is clicked.
    * */
    ngOnRegisterClicked(): void;

    /*
    * Called when forgot password form is submitted.
    * */
    ngOnForgotPasswordFormSubmitted($event: IAngularEvent): void;

    //#endregion
}