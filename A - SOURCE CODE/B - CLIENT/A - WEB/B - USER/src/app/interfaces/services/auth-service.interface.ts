import {IPromise} from "angular";

export interface IAuthService {

    //#region Methods

    // Load google api.
    loadGoogleApiLib(): IPromise<void>;

    // Load google auth api.
    loadGoogleAuthApi(): IPromise<void>;

    // Load facebook sdk.
    loadFacebookSdk(): IPromise<void>;

    // Display facebook login.
    displayFacebookLogin(): IPromise<fb.AuthResponse>;

    // Display google login.
    displayGoogleLogin(): IPromise<string>;

    // Check whether google client has been initialized.
    bIsGoogleClientInitialized(): boolean;

    // Check whether google authorize service has been initialized.
    bIsGoogleClientAuthorizeInitialized(): boolean;

    // Check whether facebook login has been initialized or not.
    bIsFacebookLoginInitialized(): boolean;

    //#endregion

}