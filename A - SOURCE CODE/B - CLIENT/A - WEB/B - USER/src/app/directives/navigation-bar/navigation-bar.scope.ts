import {IScope} from "angular";
import {User} from "../../models/entities/user";
import {UrlStateConstant} from "../../constants/url-state.constant";

export interface INavigationBarScope extends IScope{

    //#region Properties

    // User information.
    readonly profile: User | null;

    // Raise event when login is clicked.
    readonly ngClickLogin: Function | null;

    // Raise event when sign out is clicked.
    readonly ngClickSignOut: Function | null;

    // Raise event when profile is clicked.
    readonly ngClickProfile: Function | null;

    // Raise event when register is clicked.
    readonly ngClickRegister: Function | null;

    //#endregion

    //#region Methods

    // Called when login is clicked.
    ngOnLoginClicked(): void;

    // Called when sign out is clicked.
    ngOnSignOutClicked(): void;

    // Called when profile is clicked.
    ngOnProfileClicked(): void;

    // Called when register is clicked.
    ngOnRegisterClicked(): void;

    // Called when brand is clicked.
    ngOnBrandClicked(): void;

    //#endregion
}