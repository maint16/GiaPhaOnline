import {IAngularEvent, IFormController, IScope} from "angular";
import {IRecaptchaScope} from "../../../interfaces/scopes/recaptcha.scope";
import {BasicRegisterViewModel} from "../../../view-models/users/basic-register.view-model";

export interface IBasicRegisterScope extends IScope, IRecaptchaScope{

    //#region Properties

    // Model for information binding.
    basicRegisterModel: BasicRegisterViewModel;

    // Basic register form.
    basicRegisterForm: IFormController;

    //#endregion

    //#region Methods

    /*
    * Called when login is clicked.
    * */
    ngOnLoginClicked(): void;

    /*
    * Called when forgot password is clicked.
    * */
    ngOnForgotPasswordClicked(): void;

    /*
    * Called when basic register form is submitted.
    * */
    ngOnBasicRegisterFormSubmitted($event: IAngularEvent): void;

    //#endregion
}