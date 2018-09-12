import {RouterModule, Routes} from '@angular/router';
import {NgModule} from '@angular/core';
import {IsAuthorizedGuard} from '../../guards/is-authorized-guard';
import {ProfileResolve} from '../../resolves/profile.resolve';
import {AuthorizeLayoutComponent} from '../shared/authorize-layout/authorize-layout.component';
import {ManageUsersComponent} from './manage-users/manage-users.component';

//#region Route configuration

const routes: Routes = [
  {
    path: '',
    pathMatch: 'prefix',
    component: AuthorizeLayoutComponent,
    canActivate: [IsAuthorizedGuard],
    resolve: {
      profile: ProfileResolve
    },
    children: [
      {
        path: 'manage-users',
        component: ManageUsersComponent,
        pathMatch: 'full'
      },
      {
        path: '**',
        redirectTo: 'manage-users'
      }
    ]
  }
];


//#endregion

//#region Module configuration

@NgModule({
  imports: [
    RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserRouteModule {
}

//#endregion
