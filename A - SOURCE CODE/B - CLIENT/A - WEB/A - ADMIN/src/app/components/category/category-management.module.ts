import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {CalendarModule, DataTableModule, SharedModule} from "primeng/primeng";
import {MomentModule} from "angular2-moment";
import {NgxOrdinaryPagerModule} from "ngx-numeric-paginator";
import {ModalModule} from "ngx-bootstrap";
import {NgxMultiSelectorModule} from "ngx-multi-selector";
import {CategoryManagementComponent} from "./category-management.component";
import {CategorySearchBoxComponent} from "./category-search-box/category-search-box.component";
import {CategoryDetailBoxComponent} from "./category-detail-box/category-detail-box.component";
import {CategoryInitiateBoxComponent} from "./category-initiator-box/category-initiate-box.component";
import {CategoryDeleteBoxComponent} from "./category-delete-box/category-delete-box.component";
import {AccountManagementModule} from "../account/account-management.module";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,

    // Moment plugin
    MomentModule,

    // NG Prime plugins
    CalendarModule,
    DataTableModule,
    SharedModule,

    // Personal plugins
    NgxOrdinaryPagerModule,
    NgxMultiSelectorModule,

    AccountManagementModule,

    // Modal module
    ModalModule.forRoot()
  ],
  declarations: [
    CategorySearchBoxComponent,
    CategoryManagementComponent,
    CategoryDetailBoxComponent,
    CategoryInitiateBoxComponent,
    CategoryDeleteBoxComponent
  ],
  exports: [
    CategorySearchBoxComponent,
    CategoryManagementComponent,
    CategoryDetailBoxComponent,
    CategoryInitiateBoxComponent,
    CategoryDeleteBoxComponent
  ]
})

export class CategoryManagementModule {
}
