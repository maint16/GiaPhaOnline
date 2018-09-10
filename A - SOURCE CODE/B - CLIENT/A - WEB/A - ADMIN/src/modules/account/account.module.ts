import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {LoginComponent} from './login/login.component';
import {AccountRouteModule} from './account.route';
import {TranslateModule} from '@ngx-translate/core';


//#region Module declaration

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    AccountRouteModule,
    TranslateModule
  ],
  declarations: [
    LoginComponent
  ],
  exports: [
    LoginComponent
  ]
})

export class AccountModule {
}

//#endregion
