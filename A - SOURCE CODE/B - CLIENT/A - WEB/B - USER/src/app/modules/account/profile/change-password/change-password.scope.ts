import {IAngularEvent, IFormController, IScope} from "angular";
import {IModalScope} from "angular-ui-bootstrap";

export interface IChangePasswordScope extends IScope, IModalScope {

    //#region Properties

    changePasswordForm: IFormController;

    password: string;

    confirmPassword: string;

    originalPassword: string;

    //#endregion

    //#region Methods

    // Called when modal cancel is clicked.
    ngOnModalCancelClicked(): void;

    // Called when modal ok is clicked.
    ngOnPasswordSubmitted($event: IAngularEvent): void;

    //#endregion
}