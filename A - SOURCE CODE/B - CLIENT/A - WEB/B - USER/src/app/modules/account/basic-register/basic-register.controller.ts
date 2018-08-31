import {IAngularEvent, IController} from "angular";
import {IBasicRegisterScope} from "./basic-register.scope";
import {StateService} from "@uirouter/core";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {BasicRegisterViewModel} from "../../../view-models/users/basic-register.view-model";
import {IUserService} from "../../../interfaces/services/user-service.interface";

import {cloneDeep} from 'lodash';
import {IUiService} from "../../../interfaces/services/ui-service.interface";
import {IToastrService} from "angular-toastr";

/* @ngInject */
export class BasicRegisterController implements IController{

    //#region Properties

    //#endregion

    //#region Constructor

    public constructor(public $state: StateService,
                       public $ui: IUiService, public $translate: angular.translate.ITranslateService, public toastr: IToastrService,
                       public $user: IUserService,
                       public $scope: IBasicRegisterScope){

        // Properties binding.
        $scope.basicRegisterModel = new BasicRegisterViewModel();

        // Methods binding.
        $scope.ngOnLoginClicked = this._ngOnLoginClicked;
        $scope.ngOnForgotPasswordClicked = this._ngOnForgotPasswordClicked;
        $scope.ngOnBasicRegisterFormSubmitted = this._ngOnBasicRegisterFormSubmitted;
    }

    //#endregion

    //#region Methods

    /*
    * Called when login is clicked.
    * */
    private _ngOnLoginClicked = (): void => {
        // Redirect user to login page.
        this.$state.go(UrlStateConstant.loginModuleName);
    };

    /*
    * Called when forgot password is clicked.
    * */
    private _ngOnForgotPasswordClicked = (): void => {
        this.$state.go(UrlStateConstant.accountForgotPasswordModuleName);
    };

    /*
    * Called when basic register form is submitted.
    * */
    private _ngOnBasicRegisterFormSubmitted = ($event: IAngularEvent): void => {

        // Prevent default behavior.
        $event.preventDefault();

        // Form is invalid.
        if (this.$scope.basicRegisterForm.$invalid)
            return;

        // Copy model.
        let model = cloneDeep(this.$scope.basicRegisterModel);

        // Block app UI.
        this.$ui.blockAppUI();

        this.$user
            .basicRegister(model)
            .then(() => {

                // Get translation message.
                let message = this.$translate.instant('MSG_BASIC_REGISTER_SUCCESSFULLY');
                this.toastr.success(message);

                // Redirect user to login page.
                this.$state.go(UrlStateConstant.loginModuleName);
            })
            .finally(() => {
                this.$ui.unblockAppUI();
            })
    }

    //#endregion
}