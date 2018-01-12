import {Component, Inject, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {Response} from "@angular/http";
import {IAccountService} from "../../../../interfaces/services/api/account-service.interface";
import {IAuthenticationService} from "../../../../interfaces/services/authentication-service.interface";

@Component({
  selector: 'navigation-bar',
  templateUrl: 'navigation-bar.component.html'
})

export class NavigationBarComponent implements OnInit {

  //#region Properties

  // Account property.
  private account: Account;

  //#endregion

  //#region Constructor

  // Initiate instance with IoC.
  public constructor(@Inject("IAuthenticationService") public authenticationService: IAuthenticationService,
                     public router: Router,
                     public activatedRoute: ActivatedRoute) {
  }

  //#endregion

  //#region Methods

  /*
  * Sign the user out.
  * */
  public clickSignOut(): void {
    // Clear the authentication service.
    this.authenticationService.clearIdentity();

    // Re-direct to login page.
    this.router.navigate(['/login']);
  }

  /*
  * This callback is fired when this component has been initialized.
  * */
  public ngOnInit(): void {
    this.activatedRoute.data.subscribe((x: any) => {
      this.account = <Account> x.profile;
    });
  }

  //#endregion
}
