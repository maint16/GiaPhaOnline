import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NgModule} from '@angular/core';
import {SharedModule} from '../shared/shared.module';
import {AddCategoryGroupComponent} from './add-category-group.component';
import {TableModule} from 'primeng/table';
import {DataTableModule} from 'primeng/primeng';
import {AddCategoryGroupRouteModule} from './add-category-group.route';
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
    AddCategoryGroupRouteModule,
    TranslateModule
  ],
  declarations: [
    AddCategoryGroupComponent
  ],
  exports: [
    AddCategoryGroupComponent
  ]


})

export class AddCategoryGroupModule {
}

//#endregion
