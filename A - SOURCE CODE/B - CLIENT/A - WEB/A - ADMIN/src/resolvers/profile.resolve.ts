import {Inject, Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs/Observable";
import {IUserService} from "../interfaces/services/user-service.interface";

@Injectable()
export class ProfileResolve implements Resolve<Account> {

  //#region Constructor

  /*
  * Initiate resolver with injectors.
  * */
  public constructor(@Inject('IUserService') private accountService: IUserService) {
  }

  //#endregion

  //#region Methods

  /*
  * Resolve service value.
  * */
  public resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Account | Observable<Account> | Promise<Account> {
    // return this.accountService.getClientProfile().then((x: Response) => {return <Account> x.json();});
    return null;
  }

//#endregion
}
