import {IAngularEvent, IController} from "angular";
import {IChangePasswordScope} from "./change-password.scope";
import {IModalInstanceService} from 'angular-ui-bootstrap';
import {IUserService} from "../../../../interfaces/services/user-service.interface";
import {IUiService} from "../../../../interfaces/services/ui-service.interface";
import {ChangePasswordViewModel} from "../../../../view-models/users/change-password.view-model";
import {TokenViewModel} from "../../../../view-models/users/token.view-model";

export class ChangePasswordController implements IController {

    //#region Properties

    //#endregion

    //#region Constructor

    public constructor(public $scope: IChangePasswordScope,
                       public $ui: IUiService,
                       public $user: IUserService){

        $scope.password = '';
        $scope.originalPassword = '';
        $scope.confirmPassword = '';

        // Methods binding.
        $scope.ngOnModalCancelClicked = this._ngOnModalCancelClicked;
        $scope.ngOnPasswordSubmitted = this._ngOnPasswordSubmitted;
    }

    //#endregion

    //#region Methods

    // Called when cancel button is clicked.
    private _ngOnModalCancelClicked = (): void => {
        this.$scope.$dismiss();
    };

    // Called when ok button is clicked.
    private _ngOnPasswordSubmitted = ($event: IAngularEvent): void => {

        if ($event)
            $event.preventDefault();

        // Form is not valid.
        if (this.$scope.changePasswordForm.$invalid)
            return;

        // Add loading screen.
        this.$ui.blockAppUI();

        let model = new ChangePasswordViewModel();
        model.originalPassword = this.$scope.originalPassword;
        model.password = this.$scope.password;

        this.$user
            .changePassword(model)
            .then((token: TokenViewModel) => {
                // Add loading screen.
                this.$scope.$close(token);
            })
            .finally(() => {
                this.$ui.unblockAppUI();
            });
    };

    //#endregion
}