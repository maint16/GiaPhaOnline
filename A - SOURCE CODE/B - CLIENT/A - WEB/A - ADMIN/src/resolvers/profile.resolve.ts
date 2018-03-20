import {Inject, Injectable} from "@angular/core";
import {ActivatedRouteSnapshot, Resolve, RouterStateSnapshot} from "@angular/router";
import {Observable} from "rxjs/Observable";
import {IUserService} from "../interfaces/services/user-service.interface";
import {User} from "../models/entities/user";

@Injectable()
export class ProfileResolve implements Resolve<User> {

  //#region Constructor

  /*
  * Initiate resolver with injectors.
  * */
  public constructor(@Inject('IUserService') private userService: IUserService) {
  }

  //#endregion

  //#region Methods

  /*
  * Resolve service value.
  * */
  public resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): User | Observable<User> | Promise<User> {
    return this.userService.getPersonalProfile(null);
  }

//#endregion
}
