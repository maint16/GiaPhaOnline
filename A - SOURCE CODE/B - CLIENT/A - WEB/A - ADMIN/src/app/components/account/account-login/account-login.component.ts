import {Component, Inject, ViewChild} from "@angular/core";
import {Router} from "@angular/router";
import {NgForm} from "@angular/forms";
import {AuthorizationToken} from "../../../../models/authorization-token";
import {ToastrService} from "ngx-toastr";
import {BasicLoginViewModel} from "../../../../viewmodels/user/basic-login.view-model";
import {IUserService} from "../../../../interfaces/services/user-service.interface";
import {IAuthenticationService} from "../../../../interfaces/services/authentication-service.interface";
import 'rxjs/add/operator/catch';
import {Observable} from "rxjs/Observable";

@Component({
  selector: 'account-login',
  templateUrl: 'account-login.component.html'
})

export class AccountLoginComponent {

  //#region Properties

  // Whether login function is being executed or not.
  private bIsLoading: boolean;

  // Login view model.
  private loginViewModel: BasicLoginViewModel;

  // Login form group.
  @ViewChild("loginPanel")
  public loginPanel: NgForm;

  /*
  * Basic login information.
  * */
  private basicLoginInfo: BasicLoginViewModel;

  //#endregion

  //#region Constructor

  /*
  * Initiate component with default settings.
  * */
  public constructor(@Inject('IUserService') public userService: IUserService,
                     @Inject('IAuthenticationService') public authenticationService: IAuthenticationService,
                     public toastr: ToastrService,
                     public router: Router) {

    this.loginViewModel = new BasicLoginViewModel();
  }

  //#endregion

  //#region Methods

  /*
  * Callback is fired when login button is clicked.
  * */
  public clickLogin(event: Event) {

    // Prevent default behaviour.
    event.preventDefault();

    // Make component be loaded.
    this.bIsLoading = true;

    // Call service api to authenticate do authentication.
    let loginObservable = this.userService.basicLogin(this.loginViewModel)
      .subscribe((authenticationToken: AuthorizationToken) => {

        // Store authentication token into local storage.
        this.authenticationService.attachAuthorizationToken(authenticationToken);

        // Cancel loading state.
        this.bIsLoading = false;

        // Navigate user to account management page.
        this.router.navigate(['/account/management']);
      });
  }

  //#endregion
}
