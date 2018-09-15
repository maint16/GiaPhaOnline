import {Pipe, PipeTransform} from '@angular/core';
import {CategoryGroupStatus} from '../enums/category-group-status.enum';

@Pipe({name: 'toCategoryGroupStatusTitle'})
export class ToCategoryGroupStatusTitlePipe implements PipeTransform {

  //#region Methods

  // Transform user status enum to user status title.
  public transform(cgStatus: CategoryGroupStatus): string {
    switch (cgStatus){
      case CategoryGroupStatus.disabled:
        return 'TITLE_DISABLED';

      case CategoryGroupStatus.pending:
        return 'TITLE_PENDING';

      default:
        return 'TITLE_ACTIVE';
    }
  }

  //#endregion
}
