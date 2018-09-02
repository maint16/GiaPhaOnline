import {IController} from "angular";
import {IAuthService} from "../../interfaces/services/auth-service.interface";
import {IAppScope} from "./app.scope";

/* @ngInject */
export class AppController implements IController {

    //#region Properties

    public constructor(public $auth: IAuthService,
                       public $scope: IAppScope){

        // Methods binding.
        $scope.ngOnInit = this._ngOnInit;
    }

    //#endregion

    //#region Methods

    // Called when component is initialized.
    private _ngOnInit = (): void =>{
        this.$auth.loadGoogleApiLib()
            .then(() => {
                console.log('Google client library has been loaded successfully.');
                return this.$auth.loadGoogleAuthApi();
            })
            .then(() => {
                console.log('Google auth api has been loaded.');
            });
    }

    //#endregion
}