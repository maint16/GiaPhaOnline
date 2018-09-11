import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NgModule} from '@angular/core';
import {SharedModule} from '../../shared/shared.module';
import {ManageCategoryGroupComponent} from './manage-category-group.component';
import {TableModule} from 'primeng/table';
import {DataTableModule} from 'primeng/primeng';
import {ManageCategoryGroupRouteModule} from './manage-category-group.route';
import {TranslateModule} from '@ngx-translate/core';
//#region Routes declaration


//#endregion

//#region Module declaration

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    TableModule,
    DataTableModule,
    ManageCategoryGroupRouteModule,
    TranslateModule
  ],
  declarations: [
    ManageCategoryGroupComponent
  ],
  exports: [
    ManageCategoryGroupComponent
  ]


})

export class ManageCategoryGroupModule {
}

//#endregion
