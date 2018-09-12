import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {NgModule} from '@angular/core';
import {MomentModule} from 'ngx-moment';
import {TranslateModule} from '@ngx-translate/core';
import {UserRouteModule} from './user-route.module';
import {ManageUsersComponent} from './manage-users/manage-users.component';
import {NgxPaginationModule} from 'ngx-pagination';
import {TableModule} from 'primeng/table';
import {PaginatorModule} from 'primeng/paginator';
import {ModalModule} from 'ngx-bootstrap/modal';
import {UserDetailComponent} from './user-detail/user-detail.component';
import {SharedModule} from '../shared/shared.module';

//#region Module declaration

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    PaginatorModule,
    NgxPaginationModule,
    TableModule,
    TranslateModule.forChild({}),
    MomentModule,
    UserRouteModule,
    ModalModule.forRoot(),
    SharedModule
  ],
  declarations: [
    ManageUsersComponent,
    UserDetailComponent
  ],
  exports: [
    ManageUsersComponent,
    UserDetailComponent
  ],
  entryComponents: [
    UserDetailComponent
  ]
})

export class UserModule {
}

//#endregion
