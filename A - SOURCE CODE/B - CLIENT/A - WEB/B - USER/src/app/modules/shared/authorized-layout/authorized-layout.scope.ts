import {IScope} from "angular";
import {User} from "../../../models/entities/user";

export interface IAuthorizedLayoutScope extends IScope {

    //#region Properties

    // Profile of current user.
    profile: User | null;

    //#endregion

    //#region Methods

    // Called when login is clicked.
    ngOnLoginClicked(): void;

    // Called when register is clicked.
    ngOnRegisterClicked(): void;

    // Called when sign out is clicked.
    ngOnSignOutClicked(): void;

    // Called when profile is clicked.
    ngOnProfileClicked(): void;

    //#endregion
}