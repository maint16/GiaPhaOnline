import {IAccountService} from "../interfaces/services/api/account-service.interface";
import {Inject, Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {Account} from '../models/entities/account';
import {Observable} from "rxjs/Observable";
import {Response} from '@angular/http';

@Injectable()
export class ProfileResolve implements Resolve<Account> {

  //#region Constructor

  /*
  * Initiate resolver with injectors.
  * */
  public constructor(@Inject('IAccountService') private accountService: IAccountService) {
  }

  //#endregion

  //#region Methods

  /*
  * Resolve service value.
  * */
  public resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Account | Observable<Account> | Promise<Account> {
    return this.accountService.getClientProfile().then((x: Response) => {return <Account> x.json();});
  }

//#endregion
}
