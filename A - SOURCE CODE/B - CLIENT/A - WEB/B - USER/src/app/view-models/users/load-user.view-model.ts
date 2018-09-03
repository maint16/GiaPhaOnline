import {ItemStatus} from "../../enums/item-status.enum";
import {UserRole} from "../../enums/user-role.enum";
import {Pagination} from "../../models/pagination";

export class LoadUserViewModel {

    //#region Properties

    public ids: Array<number> | null = null;

    public emails: Array<string> | null = null;

    public nicknames: Array<string> | null = null;

    public statuses: Array<ItemStatus> | null = null;

    public roles: Array<UserRole> | null = null;

    public pagination: Pagination = new Pagination();

    //#endregion

}