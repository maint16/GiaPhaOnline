import {IController} from "angular";
import {IAddEditTopicScope} from "./add-edit-topic.scope";

/* @ngInject */
export class AddEditTopicController implements IController {

    //#region Properties


    //#endregion

    //#region Constructor

    /*
    * Initialize controller with injectors.
    * */
    public constructor(public $scope: IAddEditTopicScope){

    }

    //#endregion

    //#region Methods

    private _ngOnAddEditTopicClicked = (): void => {

    }

    //#endregion
}