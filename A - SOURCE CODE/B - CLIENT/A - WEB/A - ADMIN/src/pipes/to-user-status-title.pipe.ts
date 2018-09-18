import {Pipe, PipeTransform} from '@angular/core';
import {UserStatus} from '../enums/user-status.enum';

@Pipe({name: 'toUserStatusTitle'})
export class ToUserStatusTitlePipe implements PipeTransform {

  //#region Methods

  // Transform user status enum to user status title.
  public transform(status: UserStatus): string {
    switch (status){
      case UserStatus.disabled:
        return 'TITLE_DISABLED';

      case UserStatus.pending:
        return 'TITLE_PENDING';

      default:
        return 'TITLE_ACTIVE';
    }
  }

  //#endregion
}
