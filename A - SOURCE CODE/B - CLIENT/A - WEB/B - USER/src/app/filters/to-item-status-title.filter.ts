import {ItemStatus} from "../enums/item-status.enum";

export function toItemStatusTitle()  {
    return (iItemStatus: ItemStatus): string =>  {
        switch (iItemStatus){
            case ItemStatus.disabled:
                return 'TITLE_DISABLED';

            default:
                return 'TITLE_ACTIVE';
        }
    }
}