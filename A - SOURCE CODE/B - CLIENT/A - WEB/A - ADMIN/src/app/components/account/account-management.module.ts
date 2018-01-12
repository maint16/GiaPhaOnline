import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {AccountForgotPasswordComponent} from "./account-forgot-password/account-forgot-password.component";
import {AccountLoginComponent} from "./account-login/account-login.component";
import {AccountManagementComponent} from "./account-management/account-management.component";
import {AccountSearchBoxComponent} from "./account-search-box/account-search-box.component";
import {AccountSubmitPasswordComponent} from "./account-submit-password/account-submit-password.component";
import {CalendarModule, DataTableModule, SharedModule} from "primeng/primeng";
import {MomentModule} from "angular2-moment";
import {NgxOrdinaryPagerModule} from "ngx-numeric-paginator";
import {ModalModule, PaginationModule} from "ngx-bootstrap";
import {NgxMultiSelectorModule} from "ngx-multi-selector";
import {AccountProfileBoxComponent} from "./account-profile-box/account-profile-box.component";

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
    NgxOrdinaryPagerModule,
    NgxMultiSelectorModule,

    // ngx-bootstrap modules
    PaginationModule.forRoot(),

    // Modal module
    ModalModule.forRoot()
  ],
  declarations: [
    AccountForgotPasswordComponent,
    AccountLoginComponent,
    AccountManagementComponent,
    AccountSearchBoxComponent,
    AccountSubmitPasswordComponent,
    AccountProfileBoxComponent
  ],
  exports: [
    AccountForgotPasswordComponent,
    AccountLoginComponent,
    AccountManagementComponent,
    AccountSearchBoxComponent,
    AccountSubmitPasswordComponent,
    AccountProfileBoxComponent
  ]
})

export class AccountManagementModule {
}
