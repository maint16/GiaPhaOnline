import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';

import {AppComponent} from './app.component';
import {RouterModule, Routes} from "@angular/router";
import {AccountLoginComponent} from "./components/account/account-login/account-login.component";
import {IsAuthorizedGuard} from "../guards/is-authorized-guard";
import {AccountManagementComponent} from "./components/account/account-management/account-management.component";
import {AccountService} from "../services/api/account.service";
import {CategoryService} from "../services/api/category.service";
import {BrowserAnimationsModule} from "@angular/platform-browser/animations";
import {ToastrModule} from "ngx-toastr";
import {Http, HttpModule} from "@angular/http";
import {GlobalHttpInterceptor} from "../interceptors/global-http-interceptor";
import {FormsModule} from "@angular/forms";
import {AccountProfileBoxComponent} from "./components/account/account-profile-box/account-profile-box.component";
import {AuthenticationService} from "../services/authentication.service";
import {AuthorizeLayoutComponent} from "./components/shared/authorize-layout/authorize-layout.component";
import {ApiService} from "../services/api.service";
import {ITimeService} from "../interfaces/services/time-service.interface";
import {TimeService} from "../services/time.service";
import {NavigationBarComponent} from "./components/shared/navigation-bar/navigation-bar.component";
import {SideBarComponent} from "./components/shared/side-bar/side-bar.component";
import {ConfigurationService} from "../services/configuration.service";
import {CategoryManagementComponent} from "./components/category/category-management.component";
import {ProfileResolve} from "../resolvers/profile.resolve";
import {AccountForgotPasswordComponent} from "./components/account/account-forgot-password/account-forgot-password.component";
import {ConstraintService} from "../services/constraint.service";
import {AccountSubmitPasswordComponent} from "./components/account/account-submit-password/account-submit-password.component";
import {AccountManagementModule} from "./components/account/account-management.module";
import {CategoryManagementModule} from "./components/category/category-management.module";
import {MomentModule} from "angular2-moment";
import {PaginationConfig, PaginationModule} from "ngx-bootstrap";
import {ApplicationSetting} from "../constants/application-setting";

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
    path: 'category',
    component: AuthorizeLayoutComponent,
    canActivate: [IsAuthorizedGuard],
    resolve:{
      profile: ProfileResolve
    },
    children: [
      {
        path: 'management',
        component: CategoryManagementComponent,
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
    path: 'forgot-password',
    component: AccountForgotPasswordComponent,
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
    HttpModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(), // ToastrModule added
    MomentModule,

    // Application modules.
    AccountManagementModule,
    CategoryManagementModule,

    // Import router configuration.
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
    IsAuthorizedGuard,
    {provide: 'IApiService', useClass: ApiService},
    {provide: 'IAccountService', useClass: AccountService},
    {provide: 'ICategoryService', useClass: CategoryService},
    {provide: 'IAuthenticationService', useClass: AuthenticationService},
    {provide: 'ITimeService', useClass: TimeService},
    {provide: 'IConfigurationService', useClass: ConfigurationService},
    {provide: PaginationConfig, useValue: {main: {boundaryLinks: true, directionLinks: true,  firstText: '&lt;&lt;', previousText: '&lt;', nextText: '&gt;', lastText: '&gt;&gt;', itemsPerPage: ApplicationSetting.maxPageRecords, maxSize: 5}}},
    ConstraintService,

    // Handle common behaviour of http request / response.
    {provide: Http, useClass: GlobalHttpInterceptor},

    // Resolvers.
    ProfileResolve
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
