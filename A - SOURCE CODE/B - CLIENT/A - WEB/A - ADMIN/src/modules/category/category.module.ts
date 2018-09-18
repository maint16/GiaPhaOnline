import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NgModule} from '@angular/core';
import {SharedModule} from '../shared/shared.module';
import {ManageCategoryComponent} from './manage-category/manage-category.component';
import {TableModule} from 'primeng/table';
import {DataTableModule} from 'primeng/primeng';
import {TranslateModule} from '@ngx-translate/core';
import {AddCategoryComponent} from './add-category/add-category.component';
import {CategoryRouteModule} from './category.route';
import {ToCategoryStatusTitlePipe} from '../../pipes/to-category-status-title.pipe';
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
    CategoryRouteModule,
    TranslateModule
  ],
  declarations: [
    ManageCategoryComponent,
    AddCategoryComponent,
    ToCategoryStatusTitlePipe
  ],
  exports: [
    ManageCategoryComponent,
    AddCategoryComponent,
    ToCategoryStatusTitlePipe
  ]


})

export class CategoryModule {
}

//#endregion
