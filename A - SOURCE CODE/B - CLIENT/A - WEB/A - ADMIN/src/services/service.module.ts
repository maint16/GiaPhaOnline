import {NgModule, ModuleWithProviders} from '@angular/core';
import {IAccountService} from '../interfaces/services/account-service.interface';
import {AccountService} from './account.service';
import {IAuthenticationService} from '../interfaces/services/authentication-service.interface';
import {AuthenticationService} from './authentication.service';
import {UserService} from './user.service';
import {CategoryGroupService} from './category-group.service';
import {CategoryService} from './category.service';

@NgModule({})

export class ServiceModule {

  //#region Methods

  static forRoot(): ModuleWithProviders {
    return {
      ngModule: ServiceModule,
      providers: [
        {provide: 'IAccountService', useClass: AccountService},
        {provide: 'IAuthenticationService', useClass: AuthenticationService},
        {provide: 'IUserService', useClass: UserService},
        {provide: 'ICategoryGroupService', useClass: CategoryGroupService},
        {provide: 'ICategoryService', useClass: CategoryService},
        {provide: Window, useValue: window}
      ]
    };
  }

  //#endregion
}


