import {LoadUserViewModel} from "../view-models/users/load-user.view-model";
import {SearchResult} from "../models/search-result";
import {User} from "../models/entities/user";
import {IHttpResponse, IHttpService, IPromise, IRequestShortcutConfig} from "angular";
import {AppSetting} from "../models/app-setting";
import {LoginViewModel} from "../view-models/users/login.view-model";
import {TokenViewModel} from "../view-models/users/token.view-model";
import {IUserService} from "../interfaces/services/user-service.interface";
import {ForgotPasswordViewModel} from "../view-models/users/forgot-password.view-model";
import {BasicRegisterViewModel} from "../view-models/users/basic-register.view-model";
import {UserRole} from "../enums/user-role.enum";
import {UserStatus} from "../enums/user-status.enum";
import {ChangePasswordViewModel} from "../view-models/users/change-password.view-model";

/* @ngInject */
export class UserService implements IUserService {

    //#region Constructor

    /*
    * Initialize service with injectors.
    * */
    public constructor(public appSettingConstant: AppSetting,
                       public $http: IHttpService) {

    }

    //#endregion

    //#region Methods

    /*
    * Load users by using specific conditions.
    * */
    public loadUsers(conditions: LoadUserViewModel): IPromise<SearchResult<User>> {
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/user/search`;
        return this.$http
            .post(url, conditions)
            .then((loadUsersResponse: IHttpResponse<SearchResult<User>>) => {
                if (!loadUsersResponse)
                    throw 'No user has been found';

                let loadUsersResult = loadUsersResponse.data;
                if (!loadUsersResult || !loadUsersResult.records)
                    throw 'No user has been found';

                return loadUsersResult;
            });
    }

    /*
    * Basic login using information.
    * */
    public basicLogin(model: LoginViewModel): IPromise<TokenViewModel> {
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/user/basic-login`;
        return this.$http
            .post(url, model)
            .then((loginResponse: IHttpResponse<TokenViewModel>) => {
                if (!loginResponse)
                    throw 'Failed to login';

                let loginResult = loginResponse.data;
                if (!loginResult)
                    throw 'Failed to login';

                return loginResult;
            });
    }

    /*
    * Submit information to reset password.
    * */
    public forgotPassword(model: ForgotPasswordViewModel): IPromise<null> {
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/user/forgot-password`;
        return this.$http
            .post(url, model)
            .then((forgotPasswordResponse: IHttpResponse<null>) => {
                if (!forgotPasswordResponse)
                    throw 'Failed to reset password';

                return null;
            });
    };

    /*
    * Register basic account.
    * */
    public basicRegister(model: BasicRegisterViewModel): IPromise<null> {
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/user/basic-register`;
        return this.$http
            .post(url, model)
            .then((basicRegisterResponse: IHttpResponse<null>) => {
                if (!basicRegisterResponse)
                    throw 'Failed to register';

                return null;
            });
    };

    /*
    * If id is specified, user whose id match to this one will be fetched.
    * If id is not specified, requester profile is fetched.
    * */
    public loadUserProfile(id?: number): IPromise<User> {
        // Construct url.
        // Construct url.
        let url = `${this.appSettingConstant.apiEndPoint}/api/user/${id}`;
        return this.$http
            .get(url)
            .then((loadUserProfileResponse: IHttpResponse<User>) => {
                if (!loadUserProfileResponse)
                    throw 'No profile is found';

                return loadUserProfileResponse.data;
            });
    };

    // Load available statuses for user.
    public loadUserAvailableStatuses(): Array<UserStatus> {
        return [UserStatus.disabled, UserStatus.pending, UserStatus.active];
    }

    // Upload user profile image.
    public uploadProfileImage(blob: any): IPromise<string> {
        let url = `${this.appSettingConstant.apiEndPoint}/api/user/upload-avatar`;
        let options: IRequestShortcutConfig = {};
        options.headers = {};
        options.headers['Content-Type'] = undefined;

        let fd = new FormData();
        fd.set('image', blob);

        return this.$http
            .post(url, fd, options)
            .then((uploadAvatarResult: IHttpResponse<any>) => {
                if (!uploadAvatarResult)
                    throw 'Failed to upload avatar';

                let data = uploadAvatarResult.data;
                if (!data.photo)
                    throw 'Failed to upload avatar';

                return <string> data.photo;
            });
    };

    // Allow user to change his/her account password.
    public changePassword(changePasswordModel: ChangePasswordViewModel): IPromise<TokenViewModel> {
        let url = `${this.appSettingConstant.apiEndPoint}/api/user/change-password`;
        return this.$http
            .post(url, changePasswordModel)
            .then((changePasswordResponse: IHttpResponse<TokenViewModel>) => {
                if (!changePasswordResponse)
                    throw 'Failed to change password';

                let token = changePasswordResponse.data;
                if (!token)
                    throw 'Failed to change password';

                return token;
            })
    }

    // Exchange google code with system access token.
    public loginGoogle(code: string): IPromise<TokenViewModel> {
        let url = `${this.appSettingConstant.apiEndPoint}/api/user/google-login`;
        let model: any = {
            code: code
        };

        return this.$http
            .post(url, model)
            .then((loginResponse: IHttpResponse<TokenViewModel>) => {
                if (!loginResponse)
                    throw 'Failed to login';

                let token = loginResponse.data;
                if (!token)
                    throw 'Failed to login';

                return token;
            })
    }

    // Exchange facebook access token with system access token.
    public loginFacebook(fbAccessToken: string): IPromise<TokenViewModel> {

        // Initialize model.
        let model = {
            accessToken: fbAccessToken
        };

        let url = `${this.appSettingConstant.apiEndPoint}/api/user/facebook-login`;
        return this.$http
            .post(url, model)
            .then((loginFacebookResult: IHttpResponse<TokenViewModel>) => {
                if (!loginFacebookResult)
                    throw 'Failed to login facebook';

                let token = loginFacebookResult.data;
                if (!token.accessToken)
                    throw 'Failed to login facebook';

                return token;
            });
    }

    //#endregion

}