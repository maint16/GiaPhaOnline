import {IAuthService} from "../interfaces/services/auth-service.interface";
import {AppSetting} from "../models/app-setting";
import {IPromise, IQService} from "angular";
import ClientConfig = gapi.auth2.ClientConfig;

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
                        .grantOfflineAccess(null)
                        .then(function (getGoogleCredentialResponse) {
                            let code = <string> getGoogleCredentialResponse.code;
                            resolve(code);
                        })
                        .catch((exception) => {
                            reject(exception);
                        });
                });
            })
    }

    // Check whether google client has been initialized or not.
    public bIsGoogleClientAuthorizeInitialized(): boolean {
        if (!gapi)
            return false;

        return true;
    }

    // Check whether google oauth2 has been initialized.
    public bIsGoogleClientInitialized(): boolean {
        if (!gapi.auth2)
            return false;

        return true;
    }

    //#endregion
}