import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {IPromise, module} from 'angular';
import {ProfileController} from "./profile.controller";
import {User} from "../../../models/entities/user";
import {StateParams, StateService} from '@uirouter/angularjs';
import {LoadUserViewModel} from "../../../view-models/users/load-user.view-model";
import {Pagination} from "../../../models/pagination";
import {IUserService} from "../../../interfaces/services/user-service.interface";
import {SearchResult} from "../../../models/search-result";

export class ProfileModule {

    //#region Constructor

    /*
    * Initialize module with injectors.
    * */
    public constructor($stateProvider: StateProvider){
        $stateProvider.state(UrlStateConstant.profileModuleName, {
            url: UrlStateConstant.profileModuleUrl,
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => resolve(require('./profile.html')));
                });
            }],
            resolve: {
                /*
                * Load login controller.
                * */
                loadController: ($q, $ocLazyLoad) => {
                    return $q((resolve) => {
                        require.ensure([], () => {
                            // load only controller module
                            let ngModule = module('account.profile', []);

                            const {ProfileController} = require('./profile.controller.ts');
                            ngModule.controller('profileController', ProfileController);
                            $ocLazyLoad.load({name: ngModule.name});
                            resolve(ngModule.controller);
                        })
                    });
                },

                /*
                * Load profile by using id.
                * */
                profile: ($stateParams: StateParams, $state: StateService, $user: IUserService) : User | IPromise<User> => {

                    // Get profile id.
                    let profileId = parseInt($stateParams.profileId);
                    if (profileId == null)
                    // Profile is not valid.
                    if (!profileId){

                        // Get current user profile.
                        $user.loadUserProfile(profileId)
                            .then((user: User) => {
                                return user;
                            })
                            .catch(() => {
                                $state.go(UrlStateConstant.dashboardModuleName);
                            });
                        return null;
                    }

                    // Build load user conditions.
                    let loadUsersCondition = new LoadUserViewModel();
                    let pagination = new Pagination();
                    pagination.page = 1;
                    pagination.records = 1;

                    loadUsersCondition.ids = [profileId];
                    return $user.loadUsers(loadUsersCondition)
                        .then((loadUsersResult: SearchResult<User>) => {
                            let users = loadUsersResult.records;
                            if (!users)
                                throw 'No user has been found';

                            return users[0];
                        })
                        .catch(() => {
                            $state.go(UrlStateConstant.dashboardModuleName);
                            return null;
                        })
                }
            },
            controller: 'profileController',
            parent: UrlStateConstant.authorizedLayoutModuleName
        });
    }

    //#endregion

}