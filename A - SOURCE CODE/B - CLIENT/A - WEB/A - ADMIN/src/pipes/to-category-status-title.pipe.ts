import {Pipe, PipeTransform} from '@angular/core';
import {CategoryStatus} from '../enums/category-status.enum';

@Pipe({name: 'toCategoryStatusTitle'})
export class ToCategoryStatusTitlePipe implements PipeTransform {

  //#region Methods

  // Transform user status enum to user status title.
  public transform(categoryStatus: CategoryStatus): string {
    switch (categoryStatus) {
      case CategoryStatus.disabled:
        return 'TITLE_DISABLED';
      case CategoryStatus.pending:
        return 'TITLE_PENDING';
      case CategoryStatus.active:
        return 'TITLE_ACTIVE';
      default:
        return 'TITLE_ALL';
    }
  }

  //#endregion
}
