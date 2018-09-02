import {IController, IScope} from "angular";
import {IAuthorizedLayoutScope} from "./authorized-layout.scope";
import {StateService} from "@uirouter/core";
import {UrlStateConstant} from "../../../constants/url-state.constant";
import {User} from "../../../models/entities/user";
import {ILocalStorageService} from "angular-local-storage";
import {LocalStorageKeyConstant} from "../../../constants/local-storage-key.constant";

/* @ngInject */
export class AuthorizedLayoutController implements IController {

    //#region Constructors

    public constructor(public profile: User,
                       public $state: StateService, public localStorageService: ILocalStorageService,
                       public $scope: IAuthorizedLayoutScope,
                       public $rootScope: IScope) {

        // Properties binding
        $scope.profile = profile;

        // Methods binding
        $scope.ngOnLoginClicked = this._ngOnLoginClicked;
        $scope.ngOnRegisterClicked = this._ngOnRegisterClicked;
        $scope.ngOnSignOutClicked = this._ngOnSignOutClicked;
        $scope.ngOnProfileClicked = this._ngOnProfileClicked;
    }

    //#endregion

    //#region Methods

    // Called when login is clicked.
    private _ngOnLoginClicked = (): void => {
        this.$state.go(UrlStateConstant.loginModuleName);
    };

    // Called when register is clicked.
    private _ngOnRegisterClicked = (): void => {
        this.$state.go(UrlStateConstant.accountRegisterModuleName);
    };

    // Called when sign out is clicked.
    private _ngOnSignOutClicked = (): void => {
        // Clear local storage.
        this.localStorageService.remove(LocalStorageKeyConstant.accessTokenKey);

        // Re-direct user to login page.
        this.$state.go(UrlStateConstant.loginModuleName);
    };

    // Called when profile is clicked.
    private _ngOnProfileClicked = (): void => {
        // Redirect user to profile page.
        this.$state.go(UrlStateConstant.profileModuleName, {profileId: 0});
    };

    //#endregion
}