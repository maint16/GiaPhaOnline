import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NgModule} from '@angular/core';
import {MomentModule} from 'ngx-moment';
import {TranslateModule} from '@ngx-translate/core';
import {UserRouteModule} from './user-route.module';
import {ManageUsersComponent} from './manage-users/manage-users.component';
import {NgxPaginationModule} from 'ngx-pagination';
import {UserDetailModule} from './user-detail/user-detail.module';
import {TableModule} from 'primeng/table';
import {PaginatorModule} from 'primeng/paginator';

//#region Module declaration

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    PaginatorModule,
    UserDetailModule,
    NgxPaginationModule,
    TableModule,
    TranslateModule.forChild({}),
    MomentModule,
    UserRouteModule
  ],
  declarations: [
    ManageUsersComponent
  ],
  exports: [
    ManageUsersComponent
  ]
})

export class UserModule {
}

//#endregion
