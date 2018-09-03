import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {NgModule} from "@angular/core";
import {SharedModule} from "../shared/shared.module";
import {ManageUsersComponent} from './manage-users.component';
import {ManageUsersRouteModule} from './manage-users.route';

//#region Routes declaration


//#endregion

//#region Module declaration

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    SharedModule,
    ManageUsersRouteModule
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
