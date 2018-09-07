import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {NgModule} from "@angular/core";
import {SharedModule} from "../shared/shared.module";
import {ManageUsersComponent} from './manage-users.component';
import {ManageUsersRouteModule} from './manage-users.route';
import {DataTableModule} from 'primeng/primeng';
import {PaginatorModule} from 'primeng/paginator';
import {UserDetailModule} from '../user-detail/user-detail.module';
import {NgxPaginationModule} from 'ngx-pagination';
//#region Routes declaration


//#endregion

//#region Module declaration

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    ManageUsersRouteModule,
    DataTableModule,
    PaginatorModule,
    UserDetailModule,
    NgxPaginationModule
  ],
  declarations: [
    ManageUsersComponent
  ],
  exports: [
    ManageUsersComponent
  ]
  })

export class ManageUsersModule {
}

//#endregion
