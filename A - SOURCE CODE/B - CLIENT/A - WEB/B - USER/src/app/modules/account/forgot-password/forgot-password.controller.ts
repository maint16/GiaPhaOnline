import {IAngularEvent, IController} from "angular";
import {IForgotPasswordScope} from "./forgot-password.scope";
import {ForgotPasswordViewModel} from "../../../view-models/users/forgot-password.view-model";
import {StateService} from "@uirouter/core";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {cloneDeep} from 'lodash';
import {IUserService} from "../../../interfaces/services/user-service.interface";
import {IUiService} from "../../../interfaces/services/ui-service.interface";
import {IToastrService} from "angular-toastr";

/* @ngInject */
export class ForgotPasswordController implements IController {

    //#region Properties

    // Captcha widget id.
    private captchaWidgetId: string = '';

    //#endregon

    //#region Constructors

    /*
    * Initialize controller with injectors.
    * */
    public constructor(public $state: StateService, public vcRecaptchaService: any, public toastr: IToastrService,
                       public $translate: angular.translate.ITranslateService,
                       public $user: IUserService,
                       public $ui: IUiService,
                       public $scope: IForgotPasswordScope) {

        // Properties binding.
        $scope.forgotPasswordModel = new ForgotPasswordViewModel();

        // Methods binding.
        $scope.ngOnLoginClicked = this._ngOnLoginClicked;
        $scope.ngOnRegisterClicked = this._ngOnRegisterClicked;
        $scope.ngOnForgotPasswordFormSubmitted = this._ngOnForgotPasswordFormSubmitted;
        $scope.ngOnWidgetInitialized = this._ngOnWidgetInitialized;
    }

    //#endregion

    //#region Methods

    /*
    * Called when login is clicked.
    * */
    private _ngOnLoginClicked = (): void => {
        this.$state.go(UrlStateConstant.loginModuleName);
    };

    /*
    * Called when basic-register is clicked.
    * */
    private _ngOnRegisterClicked = (): void => {
        this.$state.go(UrlStateConstant.accountRegisterModuleName);
    };

    /*
    * Called when forgot password form is submitted.
    * */
    private _ngOnForgotPasswordFormSubmitted = ($event: IAngularEvent): void => {

        // Prevent default behavior.
        $event.preventDefault();

        // Block screen access.
        this.$ui.blockAppUI();

        // Copy forgot password model.
        let forgotPasswordModel = cloneDeep(this.$scope.forgotPasswordModel);
        this.$user
            .forgotPassword(forgotPasswordModel)
            .then(() => {
                // Unblock ui.
                this.$ui.unblockAppUI();

                // Display reset password success message.

                // Translate the message.
                let message = this.$translate.instant('MSG_REQUEST_RESET_PASSWORD_SUCCESSFULLY');
                this.toastr.success(message);

                // Redirect user to login page.
                this.$state.go(UrlStateConstant.loginModuleName);
            })
            .catch(() => {
                // Reload captcha.
                this.vcRecaptchaService.reload(this.captchaWidgetId);
                this.$ui.unblockAppUI();
            })

    };

    /*
    * Called when captcha is initialized.
    * */
    private _ngOnWidgetInitialized = (widgetId: string): void => {
        this.captchaWidgetId = widgetId;
    }

    //#endregion
}