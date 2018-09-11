import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NgModule} from '@angular/core';
import {ManageUsersComponent} from './manage-users.component';
import {ManageUsersRouteModule} from './manage-users.route';
import {PaginatorModule} from 'primeng/paginator';
import {UserDetailModule} from '../user-detail/user-detail.module';
import {NgxPaginationModule} from 'ngx-pagination';
import {TableModule} from 'primeng/table';
import {TranslateModule} from '@ngx-translate/core';
import {SharedModule} from '../../shared/shared.module';
//#region Routes declaration


//#endregion

//#region Module declaration

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    ManageUsersRouteModule,
    PaginatorModule,
    UserDetailModule,
    NgxPaginationModule,
    TableModule,
    TranslateModule
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
