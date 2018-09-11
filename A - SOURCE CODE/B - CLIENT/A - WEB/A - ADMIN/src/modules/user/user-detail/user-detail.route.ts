// import {RouterModule, Routes} from '@angular/router';
// import {NgModule} from '@angular/core';
// import {AuthorizeLayoutComponent} from '../shared/authorize-layout/authorize-layout.component';
// import {IsAuthorizedGuard} from '../../guards/is-authorized-guard';
// import {ProfileResolve} from '../../resolves/profile.resolve';
// import {UserDetailComponent} from './manage-users.component';
//
// //#region Route configuration
//
// const routes: Routes = [
//   {
//     path: '',
//     pathMatch: 'prefix',
//     component: AuthorizeLayoutComponent,
//     canActivate: [IsAuthorizedGuard],
//     resolve: {
//       profile: ProfileResolve
//     },
//     children: [
//       {
//         path: '',
//         component: UserDetailComponent,
//         pathMatch: 'full'
//       }
//     ]
//   }
// ];
//
//
// //#endregion
//
// //#region Module configuration
//
// @NgModule({
//   imports: [
//     RouterModule.forChild(routes)],
//   exports: [RouterModule]
// })
// export class ManageUsersRouteModule {
// }
//
// //#endregion
