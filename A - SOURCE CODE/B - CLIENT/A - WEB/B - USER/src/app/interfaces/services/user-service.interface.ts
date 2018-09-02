import {IPromise} from "angular";
import {SearchResult} from "../../models/search-result";
import {LoadUserViewModel} from "../../view-models/users/load-user.view-model";
import {User} from "../../models/entities/user";
import {LoginViewModel} from "../../view-models/users/login.view-model";
import {TokenViewModel} from "../../view-models/users/token.view-model";
import {ForgotPasswordViewModel} from "../../view-models/users/forgot-password.view-model";
import {BasicRegisterViewModel} from "../../view-models/users/basic-register.view-model";
import {UserStatus} from "../../enums/user-status.enum";
import {ChangePasswordViewModel} from "../../view-models/users/change-password.view-model";

export interface IUserService {

    //#region Methods

    /*
    * Load users using specific conditions.
    * */
    loadUsers(conditions: LoadUserViewModel): IPromise<SearchResult<User>>;

    /*
    * Exchange basic login information with access token.
    * */
    basicLogin(model: LoginViewModel): IPromise<TokenViewModel>;

    /*
    * Submit request to reset password.
    * */
    forgotPassword(model: ForgotPasswordViewModel): IPromise<null>;

    /*
    * Register basic account.
    * */
    basicRegister(model: BasicRegisterViewModel): IPromise<null>;

    /*
    * If id is specified, user whose id match to this one will be fetched.
    * If id is not specified, requester profile is fetched.
    * */
    loadUserProfile(id? : number): IPromise<User>;

    // Load available statuses for user.
    loadUserAvailableStatuses(): Array<UserStatus>;

    // Upload user profile image.
    // Returns photo url when upload is successful.
    uploadProfileImage(blob: any): IPromise<string>;

    // Allow user to change his/her account password.
    changePassword(changePasswordModel: ChangePasswordViewModel): IPromise<TokenViewModel>;

    //#endregion

}