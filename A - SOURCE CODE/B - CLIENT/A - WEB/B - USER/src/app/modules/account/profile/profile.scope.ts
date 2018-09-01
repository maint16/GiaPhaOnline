import {IScope} from "angular";
import {User} from "../../../models/entities/user";
import {UserStatus} from "../../../enums/user-status.enum";

export interface IProfileScope extends IScope {

    //#region Properties

    // User of profile.
    user: User;

    // List of available status.
    availableUserStatuses: Array<UserStatus>;

    //#endregion

    //#region Methods

    // Convert user status to status title.
    ngLoadUserStatusTitle(status: UserStatus): string;

    //#endregion

}