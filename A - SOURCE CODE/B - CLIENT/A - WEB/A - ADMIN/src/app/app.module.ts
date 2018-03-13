import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';

import {AppComponent} from './app.component';
import {RouterModule, Routes} from "@angular/router";
import {AccountLoginComponent} from "./components/account/account-login/account-login.component";
import {IsAuthorizedGuard} from "../guards/is-authorized-guard";
import {AccountManagementComponent} from "./components/account/account-management/account-management.component";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {ToastrModule} from "ngx-toastr";
import {FormsModule} from "@angular/forms";
import {AuthenticationService} from "../services/authentication.service";
import {AuthorizeLayoutComponent} from "./components/shared/authorize-layout/authorize-layout.component";
import {ITimeService} from "../interfaces/services/time-service.interface";
import {TimeService} from "../services/time.service";
import {NavigationBarComponent} from "./components/shared/navigation-bar/navigation-bar.component";
import {SideBarComponent} from "./components/shared/side-bar/side-bar.component";
import {ProfileResolve} from "../resolvers/profile.resolve";
import {ConstraintService} from "../services/constraint.service";
import {AccountSubmitPasswordComponent} from "./components/account/account-submit-password/account-submit-password.component";
import {AccountManagementModule} from "./components/account/account-management.module";
import {MomentModule} from "angular2-moment";
import {PaginationConfig, PaginationModule} from "ngx-bootstrap";
import {ApplicationSetting} from "../constants/application-setting";
import {HttpClientModule} from "@angular/common/http";
import {UserService} from "../services/user.service";

//#region Route configuration

// Config application routes.
const appRoutes: Routes = [
  {
    path: 'account',
    component: AuthorizeLayoutComponent,
    canActivate: [IsAuthorizedGuard],
    resolve:{
      profile: ProfileResolve
    },
    children: [
      {
        path: 'management',
        component: AccountManagementComponent,
        pathMatch: 'full'
      },
      {
        path: '',
        redirectTo: 'management',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'account/management'
  },
  {
    path: 'login',
    component: AccountLoginComponent,
    pathMatch: 'full'
  },
  {
    path: 'submit-password',
    component: AccountSubmitPasswordComponent,
    pathMatch: 'full'
  }
];

//#endregion

@NgModule({
  declarations: [
    // Layout
    AuthorizeLayoutComponent,

    // Shared components
    NavigationBarComponent,
    SideBarComponent,

    AppComponent
  ],
  imports: [
    FormsModule,
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(), // ToastrModule added
    MomentModule,

    // Application modules.
    AccountManagementModule,

    // Import router configuration.
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
    IsAuthorizedGuard,
    {provide: 'IUserService', useClass: UserService},
    {provide: 'IAuthenticationService', useClass: AuthenticationService},
    {provide: 'ITimeService', useClass: TimeService},
    {provide: PaginationConfig, useValue: {main: {boundaryLinks: true, directionLinks: true,  firstText: '&lt;&lt;', previousText: '&lt;', nextText: '&gt;', lastText: '&gt;&gt;', itemsPerPage: ApplicationSetting.maxPageRecords, maxSize: 5}}},
    ConstraintService,

    // Resolvers.
    ProfileResolve
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
