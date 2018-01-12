import {Component, Inject} from "@angular/core";
import {Router} from "@angular/router";
import {Response} from "@angular/http";
import {SubmitPasswordViewModel} from "../../../../viewmodels/accounts/submit-password.view-model";
import {IAccountService} from "../../../../interfaces/services/api/account-service.interface";
import {ToastrService} from "ngx-toastr";
import {ConstraintService} from "../../../../services/constraint.service";

@Component({
  selector: 'account-submit-password',
  templateUrl: 'account-submit-password.component.html'
})

export class AccountSubmitPasswordComponent {

  //#region Properties

  /*
  * Whether component is busy or not.
  * */
  private bIsBusy: boolean;

  /*
  * Initiate change account password submit model.
  * */
  private information: SubmitPasswordViewModel;

  //#endregion

  //#region Constructor

  /*
  * Initiate component with injectors.
  * */
  public constructor(@Inject("IAccountService") private accountService: IAccountService,
                     private toastr: ToastrService,
                     private constraintService: ConstraintService,
                     private router: Router) {
    this.information = new SubmitPasswordViewModel();
  }

  //#endregion

  //#region Methods

  /*
  * This callback is fired when submit button is clicked.
  * */
  public clickSubmitPassword(event: any): void {

    // Make component be busy.
    this.bIsBusy = true;

    // Prevent default behavior.
    event.preventDefault();

    // Call service to change password.
    this.accountService.submitPasswordReset(this.information)
      .then((response: Response) => {
        // Tell user password has been changed successfully.
        this.toastr.success('SUBMIT_PASSWORD_SUCCESSFULLY', 'System message');

        // Redirect user to login page.
        this.router.navigate(['/']);

        // Cancel busy state.
        this.bIsBusy = false;
      })
      .catch((response: Response) => {
        // Proceed common response.
        this.bIsBusy = false;
      });
  }

  //#endregion


}
