import {StateProvider} from "@uirouter/angularjs";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {IPromise, module} from 'angular';
import {User} from "../../../models/entities/user";
import {StateParams, StateService} from '@uirouter/angularjs';
import {LoadUserViewModel} from "../../../view-models/users/load-user.view-model";
import {Pagination} from "../../../models/pagination";
import {IUserService} from "../../../interfaces/services/user-service.interface";
import {SearchResult} from "../../../models/search-result";
import {Ng1ViewDeclaration} from "@uirouter/angularjs/lib/interface";

/* @ngInject */
export class ProfileModule {

    //#region Constructor

    /*
    * Initialize module with injectors.
    * */
    public constructor($stateProvider: StateProvider) {

        //#region Views declaration

        let views: { [name: string]: Ng1ViewDeclaration } = {};

        // Master layout view.
        views[''] = {
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => {

                        require('./profile.scss');
                        require('ui-cropper/compile/unminified/ui-cropper.css');
                        resolve(require('./profile.html'));
                    });
                });
            }],
            controller: 'profileController'
        };

        views[`${UrlStateConstant.profileTopicsModuleName}@${UrlStateConstant.profileModuleName}`] = {
            templateProvider: ['$q', ($q) => {
                // We have to inject $q service manually due to some reasons that ng-annotate cannot add $q service in production mode.
                return $q((resolve) => {
                    // lazy load the view
                    require.ensure([], () => {
                        resolve(require('./topics/topics.html'));
                    });
                });
            }],
            controller: 'profileTopicsController'
        };


        //#endregion

        $stateProvider
            .state(UrlStateConstant.profileModuleName, {
                url: UrlStateConstant.profileModuleUrl,
                views: views,
                resolve: {

                    // Load master layout controller.
                    loadMasterLayoutController: ($q, $ocLazyLoad) => {
                        return $q((resolve) => {
                            require.ensure([], () => {

                                // Import module.
                                require('angular-file-upload');
                                require('ui-cropper');

                                // load only controller module
                                let ngModule = module('account.profile', ['angularFileUpload', 'uiCropper']);

                                const {ProfileController} = require('./profile.controller.ts');
                                ngModule.controller('profileController', ProfileController);

                                const {ChangePasswordController} = require('./change-password/change-password.controller.ts');
                                ngModule.controller('changePasswordController', ChangePasswordController);
                                $ocLazyLoad.load({name: ngModule.name});
                                resolve(ngModule.controller);
                            })
                        });
                    },

                    // Load personal topics controller.
                    loadPersonalTopicsController: ($q, $ocLazyLoad) => {
                        return $q((resolve) => {
                            require.ensure([], () => {
                                // load only controller module
                                let ngModule = module('account.profile.topics', []);

                                const {PersonalTopicsController} = require('./topics/topics.controller.ts');
                                ngModule.controller('profileTopicsController', PersonalTopicsController);
                                $ocLazyLoad.load({name: ngModule.name});
                                resolve(ngModule.controller);
                            })
                        });
                    },

                    /*
                    * Load profile by using id.
                    * */
                    profile: ($stateParams: StateParams, $state: StateService, $user: IUserService): User | IPromise<User> => {

                        // Get profile id.
                        let profileId = parseInt($stateParams.profileId);
                        if (profileId == null)
                        // Profile is not valid.
                            if (!profileId) {

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
                parent: UrlStateConstant.authorizedLayoutModuleName
            });
    }

    //#endregion

}