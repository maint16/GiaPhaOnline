import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NgModule} from '@angular/core';
import {SharedModule} from '../shared/shared.module';
import {ManageCategoryGroupComponent} from './manage-category-group/manage-category-group.component';
import {TableModule} from 'primeng/table';
import {DataTableModule} from 'primeng/primeng';
import {TranslateModule} from '@ngx-translate/core';
import {AddCategoryGroupComponent} from './add-category-group/add-category-group.component';
import {CategoryGroupRouteModule} from './category-group.route';
import {ToCategoryGroupStatusTitlePipe} from '../../pipes/to-category-group-status-title.pipe';
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
    CategoryGroupRouteModule,
    TranslateModule
  ],
  declarations: [
    ManageCategoryGroupComponent,
    AddCategoryGroupComponent,
    ToCategoryGroupStatusTitlePipe
  ],
  exports: [
    ManageCategoryGroupComponent,
    AddCategoryGroupComponent,
    ToCategoryGroupStatusTitlePipe
  ]


})

export class CategoryGroupModule {
}

//#endregion
