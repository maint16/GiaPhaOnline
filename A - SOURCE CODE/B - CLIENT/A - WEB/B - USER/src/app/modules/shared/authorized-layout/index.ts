import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {module} from 'angular';
import {ILocalStorageService} from "angular-local-storage";
import {LocalStorageKeyConstant} from "../../../constants/local-storage-key.constant";
import {TokenViewModel} from "../../../view-models/users/token.view-model";
import {IUserService} from "../../../interfaces/services/user-service.interface";
import {User} from "../../../models/entities/user";

/* @ngInject */
export class AuthorizedLayoutModule {

    //#region Constructor

    /*
    * Initialize module with injectors.
    * */
    public constructor($stateProvider: StateProvider){
        // State configuration
        $stateProvider.state(UrlStateConstant.authorizedLayoutModuleName, {
            controller: 'authorizedLayoutController',
            abstract: true,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => {
                        require('./authorized-layout.scss');
                        resolve(require('./authorized-layout.html'))
                    });
                });
            }],
            resolve: {

                /*
                * Load authorized-layout controller.
                * */
                loadController: ($q, $ocLazyLoad) => {
                    return $q((resolve) => {
                        require.ensure([], () => {
                            // load only controller module
                            let ngModule = module('shared.authorized-layout', []);
                            const {AuthorizedLayoutController} = require('./authorized-layout.controller.ts');
                            ngModule.controller('authorizedLayoutController', AuthorizedLayoutController);
                            $ocLazyLoad.load({name: ngModule.name});
                            resolve(ngModule.controller);
                        })
                    });
                },

                /*
                * Load requester profile.
                * */
                profile(localStorageService: ILocalStorageService,
                        $user: IUserService) {

                    // No access token has been found.
                    const token = localStorageService.get<TokenViewModel>(LocalStorageKeyConstant.accessTokenKey);
                    if (!token)
                        return null;

                    // No access token has been defined.
                    let accessToken = token.accessToken;
                    if (!accessToken || accessToken.length < 1)
                        return null;

                    // Load profile.

                    return $user.loadUserProfile()
                        .then((user: User) => {
                            return user;
                        });
                },


            }
        });
    }

    //#endregion
}