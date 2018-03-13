import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {AccountLoginComponent} from "./account-login/account-login.component";
import {AccountManagementComponent} from "./account-management/account-management.component";
import {AccountSearchBoxComponent} from "./account-search-box/account-search-box.component";
import {AccountSubmitPasswordComponent} from "./account-submit-password/account-submit-password.component";
import {CalendarModule, DataTableModule, SharedModule} from "primeng/primeng";
import {MomentModule} from "angular2-moment";
import {ModalModule, PaginationModule} from "ngx-bootstrap";
import {NgxMultiSelectorModule} from 'ngx-multi-selector';

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

    // redplane plugins.
    NgxMultiSelectorModule,

    // ngx-bootstrap modules
    PaginationModule.forRoot(),

    // Modal module
    ModalModule.forRoot()
  ],
  declarations: [
    AccountLoginComponent,
    AccountManagementComponent,
    AccountSearchBoxComponent,
    AccountSubmitPasswordComponent
  ],
  exports: [
    AccountLoginComponent,
    AccountManagementComponent,
    AccountSearchBoxComponent,
    AccountSubmitPasswordComponent
  ]
})

export class AccountManagementModule {
}
