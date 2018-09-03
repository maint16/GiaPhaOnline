import {UserStatus} from "../enums/user-status.enum";

export function toUserStatus()  {
    return (iUserStatus: UserStatus): string =>  {
        switch (iUserStatus){
            case UserStatus.pending:
                return 'TITLE_PENDING';
            case UserStatus.disabled:
                return 'TITLE_DISABLED';
            default:
                return 'TITLE_ACTIVE';
        }
    }
}