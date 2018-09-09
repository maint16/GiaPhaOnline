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
      }
    ]
  },
  {
    path: 'manage-category-group',
    canActivate : [IsAuthorizedGuard],
    children: [
      {
        path: '',
        loadChildren: 'modules/manage-category-group/manage-category-group.module#ManageCategoryGroupModule',
        canActivate : [IsAuthorizedGuard]
      },
      {
        path: 'category-group',
        loadChildren: 'modules/add-category-group/add-category-group.module#AddCategoryGroupModule',
        canActivate : [IsAuthorizedGuard]
      },
      {
        path: 'category-group/:id',
        loadChildren: 'modules/add-category-group/add-category-group.module#AddCategoryGroupModule',
        canActivate : [IsAuthorizedGuard]
      }
    ]
  },
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
