import {NgModule} from '@angular/core';
import {AppComponent} from './app.component';
import {RouterModule, Routes} from "@angular/router";
import {IsAuthorizedGuard} from '../guards/is-authorized-guard';

//#region Properties

// Application routes configuration.
export const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: '/login'
      },
      {
        path: 'dashboard',
        loadChildren: 'modules/dashboard/dashboard.module#DashboardModule',
        canActivate : [IsAuthorizedGuard]
      },
      {
        path: 'login',
        loadChildren: 'modules/account/account.module#AccountModule'
      },
      {
        path: 'manage-users',
        loadChildren: 'modules/manage-users/manage-users.module#ManageUsersModule',
        canActivate : [IsAuthorizedGuard]
      },
    ]
  }
];

//#endregion

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    RouterModule.forRoot(routes, {enableTracing: false})
  ],
  exports:[
    RouterModule
  ],
  bootstrap: [AppComponent]
})

export class AppRouteModule {
}
