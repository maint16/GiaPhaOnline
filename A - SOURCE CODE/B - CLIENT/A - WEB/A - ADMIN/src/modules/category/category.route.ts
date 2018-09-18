import {RouterModule, Routes} from '@angular/router';
import {NgModule} from '@angular/core';
import {AuthorizeLayoutComponent} from '../shared/authorize-layout/authorize-layout.component';
import {IsAuthorizedGuard} from '../../guards/is-authorized-guard';
import {ProfileResolve} from '../../resolves/profile.resolve';
import {ManageCategoryComponent} from './manage-category/manage-category.component';
import {AddCategoryComponent} from './add-category/add-category.component';

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
        path: 'manage',
        component: ManageCategoryComponent,
        pathMatch: 'full'
      },
      {
        path: 'add-new',
        component: AddCategoryComponent,
        pathMatch: 'full'
      },
      {
        path: ':id',
        component: AddCategoryComponent,
        pathMatch: 'full'
      },
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
export class CategoryRouteModule {
}

//#endregion
