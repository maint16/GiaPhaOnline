import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {NgModule} from "@angular/core";
import {SharedModule} from "../shared/shared.module";
import {UserDetailComponent} from './user-detail.component';
// import {ManageUsersRouteModule} from './user-detail.route';
import {TableModule} from 'primeng/table';
import {DataTableModule} from 'primeng/primeng';
import {TranslateModule} from '@ngx-translate/core';
//#region Routes declaration


//#endregion

//#region Module declaration

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    // ManageUsersRouteModule,
    TableModule,
    DataTableModule,
    TranslateModule
  ],
  declarations: [
    UserDetailComponent
  ],
  exports: [
    UserDetailComponent
  ]


})

export class UserDetailModule {
}

//#endregion
