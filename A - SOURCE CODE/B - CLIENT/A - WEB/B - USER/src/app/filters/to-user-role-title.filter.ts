import {UserRole} from "../enums/user-role.enum";

export function toUserRoleTitle()  {
    return (iUserRole: UserRole): string =>  {
        switch (iUserRole){
            case UserRole.admin:
                return 'TITLE_ADMIN';

            default:
                return 'TITLE_USER';
        }
    }
}