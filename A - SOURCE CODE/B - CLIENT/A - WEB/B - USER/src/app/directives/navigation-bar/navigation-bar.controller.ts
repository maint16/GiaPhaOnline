import {IController} from "angular";
import {INavigationBarScope} from "./navigation-bar.scope";
import {StateService} from "@uirouter/core";
import {UrlStateConstant} from "../../constants/url-state.constant";

/* @ngInject */
export class NavigationBarController implements IController {

    //#region Constructor

    /*
    * Initialize controller with injectors.
    * */
    public constructor(public $scope: INavigationBarScope, public $state: StateService) {
        $scope.ngOnLoginClicked = this._ngOnLoginClicked;
        $scope.ngOnRegisterClicked = this._ngClickRegister;
        $scope.ngOnSignOutClicked = this._ngClickSignOut;
        $scope.ngOnProfileClicked = this._ngClickProfile;
        $scope.ngOnBrandClicked = this._ngOnBrandClicked;
    }

    //#endregion

    //#region Methods

    // Called when login is clicked.
    private _ngOnLoginClicked = () => {
        if (!this.$scope.ngClickLogin)
            return;

        this.$scope.ngClickLogin();
    };

    // Called when register is clicked.
    private _ngClickRegister = () => {
        if (!this.$scope.ngClickRegister)
            return;

        this.$scope.ngClickRegister();
    };

    // Called when sign out is clicked.
    private _ngClickSignOut = () => {
        if (!this.$scope.ngClickSignOut)
            return;

        this.$scope.ngClickSignOut();
    };

    // Called when profile is clicked.
    private _ngClickProfile = () => {
        if (!this.$scope.ngClickProfile)
            return;

        this.$scope.ngClickProfile();
    };

    // Called when profile is clicked.
    private _ngOnBrandClicked = () => {
        // Redirect user to home page.
        this.$state.go(UrlStateConstant.dashboardModuleName);
    };

    //#endregion

}