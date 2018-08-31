import {IController} from "angular";
import {ILoginScope} from "./login.scope";
import {LoginViewModel} from "../../../view-models/users/login.view-model";
import {IUserService} from "../../../interfaces/services/user-service.interface";

import {cloneDeep} from 'lodash'
import {TokenViewModel} from "../../../view-models/users/token.view-model";
import {IUiService} from "../../../interfaces/services/ui-service.interface";
import {StateService} from '@uirouter/core';
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {ILocalStorageService} from "angular-local-storage";
import {LocalStorageKeyConstant} from "../../../constants/local-storage-key.constant";

/* @ngInject */
export class LoginController implements IController {

    //#region Properties



    //#endregion

    //#region Constructor

    /*
    * Initialize controller with injectors.
    * */
    public constructor(public $scope: ILoginScope,
                       public $state: StateService, public localStorageService: ILocalStorageService,
                       public $ui: IUiService,
                       public $user: IUserService){

        // Properties binding.
        $scope.loginModel = new LoginViewModel();

        // Methods binding.
        $scope.ngOnLoginClicked = this._ngOnLoginClicked;
        $scope.ngOnForgotPasswordClicked = this._ngOnForgotPasswordClicked;
        $scope.ngOnRegisterClicked = this._ngOnRegisterClicked;
    }

    //#endregion

    //#region Methods

    /*
    * Called when login button is clicked.
    * */
    private _ngOnLoginClicked = (): void => {

        // Form is invalid.
        if (!this.$scope.loginForm.$valid)
            return;

        // Block app UI.
        this.$ui.blockAppUI();

        // Copy login model.
        let loginModel = cloneDeep(this.$scope.loginModel);
        this.$user
            .basicLogin(loginModel)
            .then((token: TokenViewModel) => {

                // Save access token to local storage.
                this.localStorageService.set<TokenViewModel>(LocalStorageKeyConstant.accessTokenKey, token);

                // Unblock UI.
                this.$ui.unblockAppUI();

                // Redirect to dashboard.
                this.$state.go(UrlStateConstant.dashboardModuleName);
            })
            .catch(() => {
                this.$ui.unblockAppUI();
            });
    };

    /*
    * Called when forgot password is clicked.
    * */
    private _ngOnForgotPasswordClicked = (): void => {
        this.$state.go(UrlStateConstant.accountForgotPasswordModuleName);
    };

    /*
    * Called when basic-register is clicked.
    * */
    private _ngOnRegisterClicked = (): void => {
        this.$state.go(UrlStateConstant.accountRegisterModuleName);
    }

    //#endregion
}