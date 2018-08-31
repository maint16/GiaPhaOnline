import {ItemStatus} from "../../enums/item-status.enum";
import {UserRole} from "../../enums/user-role.enum";

export class User {
    public id: number = 0;

    public email: string = '';

    public nickname: string = '';

    public photo: string = '';

    public status: ItemStatus = ItemStatus.available;

    public role: UserRole = UserRole.ordinary;

    public joinedTime: number = 0;

    public lastModifiedTime: number | null = null;
}