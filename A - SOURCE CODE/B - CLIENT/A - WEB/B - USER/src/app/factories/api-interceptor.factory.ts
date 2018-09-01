import {IHttpInterceptor, IHttpResponse, IPromise, IQService, IRequestConfig} from "angular";
import {ILocalStorageService} from "angular-local-storage";
import {TokenViewModel} from "../view-models/users/token.view-model";
import {LocalStorageKeyConstant} from "../constants/local-storage-key.constant";
import IInjectorService = angular.auto.IInjectorService;
import {StateService} from "@uirouter/core";
import {IToastrService} from "angular-toastr";
import {UrlStateConstant} from "../constants/url-state.constant";


export class ApiInterceptorFactory implements IHttpInterceptor {

    //#region Methods

    // Trigger when request is being prepared.
    request = (httpRequestConfiguration: IRequestConfig): IRequestConfig | IPromise<IRequestConfig> => {
        // Turn on loading screen.
        //blockUI.start();

        // Get token.
        let authenticationToken = this.localStorageService.get<TokenViewModel>(LocalStorageKeyConstant.accessTokenKey);
        if (!authenticationToken)
            return httpRequestConfiguration;

        let httpRequestHeaders = httpRequestConfiguration.headers;
        httpRequestHeaders['Authorization'] = `Bearer ${authenticationToken.accessToken}`;
        return httpRequestConfiguration;
    };

    // Trigger when response is not successful.
    responseError = (rejection: any): IPromise<IHttpResponse<any>> | IHttpResponse<any> => {
        // Response is invalid.
        if (!rejection)
            return this.$q.reject(rejection);

        const url = rejection.config.url;
        if (!url || url.indexOf('/api/') === -1)
            return this.$q.reject(rejection);

        // Get service.
        let szMessage = '';

        // Get injected services.
        const toastr = <IToastrService> this.$injector.get('toastr');
        const $state = <StateService> this.$injector.get('$state');
        const $translate = <angular.translate.ITranslateService> this.$injector.get('$translate');

        switch (rejection.status) {
            case 401:
                const szAuthenticateError = rejection.headers('WWW-Authenticate');

                // Token is invalid.
                if (szAuthenticateError.indexOf('invalid_token')) {
                    // Clear token from local storage.
                    this.localStorageService.remove(LocalStorageKeyConstant.accessTokenKey);

                    // Redirect user to dashboard page.
                    $state.go(UrlStateConstant.dashboardModuleName);
                }
                szMessage = 'MSG_CREDENTIAL_INVALID';
                break;
            case 500:
                szMessage = 'MSG_INTERNAL_SERVER_ERROR';
                break;
            default:
                szMessage = 'MSG_UNKNOWN_ERROR';
                break;
        }

        let translatedMessage = $translate.instant(szMessage);
        toastr.error(translatedMessage);
        return this.$q.reject(rejection);
    };

    //#endregion

    //#region Constructor

    // Initialize interceptor with settings.
    public constructor(public localStorageService: ILocalStorageService,
                       public $q: IQService,
                       public $injector: IInjectorService) {
    }

    //#endregion
}