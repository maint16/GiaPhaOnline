import {IController, IWindowService} from "angular";
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
import {IAuthService} from "../../../interfaces/services/auth-service.interface";
import {IToastrService} from "angular-toastr";

/* @ngInject */
export class LoginController implements IController {

    //#region Properties



    //#endregion

    //#region Constructor

    /*
    * Initialize controller with injectors.
    * */
    public constructor(public $scope: ILoginScope,
                       public $state: StateService, public localStorageService: ILocalStorageService, public $window: IWindowService,
                       public $translate: angular.translate.ITranslateService, public toastr: IToastrService,
                       public $ui: IUiService, public $auth: IAuthService,
                       public $user: IUserService){

        // Properties binding.
        $scope.loginModel = new LoginViewModel();

        // Methods binding.
        $scope.ngOnLoginClicked = this._ngOnLoginClicked;
        $scope.ngOnForgotPasswordClicked = this._ngOnForgotPasswordClicked;
        $scope.ngOnRegisterClicked = this._ngOnRegisterClicked;
        $scope.ngOnGoogleLoginClicked = this._ngOnGoogleLoginClicked;
        $scope.ngOnFacebookLoginClicked = this._ngOnFacebookLoginClicked;
        $scope.ngIsAbleToLoginGoogle = this._ngIsAbleToLoginGoogle;
        $scope.ngIsAbleToLoginFacebook = $auth.bIsFacebookLoginInitialized;
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
    };

    // Called when google login is clicked.
    private _ngOnGoogleLoginClicked = (): void => {

        // Add loading screen.
        this.$ui.blockAppUI();

        // Display google login.
        this.$auth.displayGoogleLogin()
            .then((idToken: string) => {
                // Exchange google code with system access token.
                return this.$user
                    .loginGoogle(idToken);
            })
            .then((token: TokenViewModel) => {
                // Update local storage.
                this.localStorageService.set<TokenViewModel>(LocalStorageKeyConstant.accessTokenKey, token);

                // Redirect user to dashboard.
                this.$state.go(UrlStateConstant.dashboardModuleName);

                this.$ui.unblockAppUI();
            })
            .catch((error) => {
                console.log(error);
                // Get translated message.
                let message = this.$translate.instant('MSG_ERROR_HAPPENED_WHILE_SIGN_IN_GOOGLE');
                this.toastr.error(message);
                this.$ui.unblockAppUI();
            });
    };

    // Called when facebook login is clicked.
    private _ngOnFacebookLoginClicked = (): void => {

        // Add loading screen.
        this.$ui.blockAppUI();

        // Display facebook login.
        this.$auth.displayFacebookLogin()
            .then((authResponse: fb.AuthResponse) => {
                return this.$user
                    .loginFacebook(authResponse.accessToken);
            })
            .then((token: TokenViewModel) => {
                // Update local storage.
                this.localStorageService.set<TokenViewModel>(LocalStorageKeyConstant.accessTokenKey, token);

                // Redirect user to dashboard.
                this.$state.go(UrlStateConstant.dashboardModuleName);

                this.$ui.unblockAppUI();
            })
            .catch((error) => {

                // Display error message.
                console.log(error);

                // Get translated message.
                let message = this.$translate.instant('MSG_ERROR_HAPPENED_WHILE_SIGN_IN_FACEBOOK');
                this.toastr.error(message);
                this.$ui.unblockAppUI();
            });
    };

    // Check whether user can do google login or not.
    private _ngIsAbleToLoginGoogle = (): boolean => {

        // gapi hasn't been loaded.
        if (!this.$auth.bIsGoogleClientAuthorizeInitialized())
            return false;

        if (!this.$auth.bIsGoogleClientInitialized())
            return false;

        return true;
    }

    //#endregion
}