import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {NgModule} from "@angular/core";
import {SharedModule} from "../shared/shared.module";
import {UserDetailComponent} from './user-detail.component';
// import {ManageUsersRouteModule} from './user-detail.route';
import {TableModule} from 'primeng/table';
import {DataTableModule} from 'primeng/primeng';
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
    DataTableModule
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
