import {NgModule} from '@angular/core';
import {AppComponent} from './app.component';
import {RouterModule, Routes} from '@angular/router';
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
        path: 'user',
        loadChildren: 'modules/user/user.module#UserModule',
        canActivate : [IsAuthorizedGuard]
      },
      {
        path: 'category-group',
        loadChildren: 'modules/category-group/category-group.module#CategoryGroupModule',
canActivate : [IsAuthorizedGuard]
      },
      {
        path: 'category',
        loadChildren: 'modules/category/category.module#CategoryModule',
        canActivate : [IsAuthorizedGuard]
      }
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
