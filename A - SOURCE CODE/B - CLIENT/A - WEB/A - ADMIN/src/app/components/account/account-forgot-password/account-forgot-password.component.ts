import {Inject, Component} from '@angular/core';
import {Response} from '@angular/http';
import {Router} from '@angular/router';
import {Account} from '../../../../models/entities/account';
import {IAccountService} from "../../../../interfaces/services/api/account-service.interface";
import {ToastrService} from "ngx-toastr";
import {ConstraintService} from "../../../../services/constraint.service";

@Component({
  selector: 'account-forgot-password-box',
  templateUrl: 'account-forgot-password.component.html'
})

export class AccountForgotPasswordComponent {

  //#region Properties

  /*
  * Account information.
  * */
  private account: Account;

  /*
  * Whether component is busy or not.
  * */
  private bIsBusy: boolean;

  //#endregion

  //#region Constructor

  /*
  * Initiate component with default settings.
  * */
  public constructor(@Inject('IAccountService') private accountService: IAccountService,
                     public constraintService: ConstraintService,
                     private toastr: ToastrService,
                     private router: Router) {

    // Initiate account instance.
    this.account = new Account();

    // Set loading to be false.
    this.bIsBusy = false;
  }

  //#endregion

  //#region Methods

  /*
  * Callback which is fired when seach button is clicked for requesting a password reset.
  * */
  public clickRequestPassword(event: any): void {

    // Set component to loading state.
    this.bIsBusy = true;

    // Prevent default behaviour.
    event.preventDefault();

    // Call api to request password change.
    this.accountService.initChangePasswordRequest(this.account.email)
      .then((response: Response) => {

        // Tell client that password request has been submitted.
        this.toastr.success('CHANGE_PASSWORD_REQUEST_SUBMITTED', 'System message');

        // Redirect user to submit password page.
        this.router.navigate(['/submit-password']);

        // Cancel component loading state.
        this.bIsBusy = false;
      })
      .catch((response: Response) => {
        // Cancel component loading state.
        this.bIsBusy = false;
      });
  }

  //#endregion
}
