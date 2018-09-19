import {IAuthService} from "../interfaces/services/auth-service.interface";
import {AppSetting} from "../models/app-setting";
import {IPromise, IQService, IWindowService} from "angular";
import ClientConfig = gapi.auth2.ClientConfig;
import GoogleUser = gapi.auth2.GoogleUser;

declare const FB: any;
declare const $: JQueryStatic;

/* @ngInject */
export class AuthService implements IAuthService {

    //#region Properties


    //#endregion

    //#region Constructor

    public constructor(public appSettingConstant: AppSetting,
                       public $q: IQService) {

    }

    //#endregion

    //#region Methods

    // Load google api library.
    public loadGoogleApiLib(): IPromise<void> {
        return this
            .$q((resolve, reject) => {
                const szSdkName: string = 'google-jssdk';

                // Sdk has been imported before. Prevent it from being imported again.
                if (document.getElementById(szSdkName)) {
                    resolve();
                    return;
                }

                const apiGoogle = document.createElement('script');
                apiGoogle.id = szSdkName;
                apiGoogle.type = 'text/javascript';
                apiGoogle.async = true;
                apiGoogle.src = 'https://apis.google.com/js/platform.js';
                apiGoogle.onload = () => {
                    resolve();
                };

                apiGoogle.onerror = () => {
                    reject('gapi hasn\'t been loaded successfully.');
                };

                document.head.appendChild(apiGoogle);
            });
    }

    // Load Google auth api package.
    public loadGoogleAuthApi(): IPromise<void> {
        return this.$q(resolve => {
            if (gapi.auth2)
                resolve();

            gapi.load('auth2', () => {
                const params: ClientConfig = {
                    client_id: this.appSettingConstant.clientIdGoogle,
                    scope: this.appSettingConstant.clientScopeGoogle,
                    fetch_basic_profile: true
                };
                gapi.auth2.init(params);
                resolve();
            });
        })
    }

    // Load facebook sdk.
    public loadFacebookSdk(): IPromise<void> {
        return this.$q((resolve, reject) => {

            if (this.bIsFacebookLoginInitialized()) {
                resolve();
                return;
            }

            $.ajaxSetup({cache: true});
            $.getScript('https://connect.facebook.net/en_US/sdk.js',
                () => {
                    FB.init({
                        appId: this.appSettingConstant.clientIdFacebook,
                        version: 'v2.12' // or v2.1, v2.2, v2.3, ...
                    });
                    resolve();
                })
                .fail(() => {
                    reject();
                });
        });
    };

    // Display google login.
    public displayFacebookLogin(): IPromise<fb.AuthResponse> {
        // Add facebook sdk, in case of its unavailability.
        return this.loadFacebookSdk()
            .then(() => {
                return this.$q<fb.AuthResponse>((resolve, reject) => {
                    FB.login((response => {
                        if (response.status === 'connected') {
                            resolve(response.authResponse);
                            return;
                        }

                        reject(response.status);
                    }), {scope: this.appSettingConstant.clientScopeFacebook})
                });

            });
    }

    // Display google login.
    public displayGoogleLogin(): IPromise<string> {
        // Load Google API.
        return this.loadGoogleApiLib()
            .then(() => {
                return this.loadGoogleAuthApi();
            })
            .then(() => {
                return this.$q<string>((resolve, reject) => {
                    gapi.auth2
                        .getAuthInstance()
                        .signIn()
                        .then( (googleUser: GoogleUser) => {
                            let authResponse = googleUser.getAuthResponse();
                            resolve(authResponse.id_token);
                        })
                        .catch((exception) => {
                            reject(exception);
                        });
                });
            })
    }

    // Check whether google client has been initialized or not.
    public bIsGoogleClientAuthorizeInitialized(): boolean {
        try {
            if (!gapi)
                return false;

            return true;
        } catch {
            return false;
        }
    }

    // Check whether google oauth2 has been initialized.
    public bIsGoogleClientInitialized(): boolean {
        try {
            if (!gapi.auth2)
                return false;

            return true;
        } catch {
            return false;
        }
    }

    // Check whether facebook login has been initialized or not.
    public bIsFacebookLoginInitialized(): boolean{
        try {
            if (!FB)
                return false;

            return true;
        } catch {
            return false;
        }
    }

    //#endregion
}