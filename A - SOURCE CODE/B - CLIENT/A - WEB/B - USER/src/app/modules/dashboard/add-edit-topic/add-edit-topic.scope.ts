import {IAngularEvent, IFormController, IScope} from "angular";
import {Topic} from "../../../models/entities/topic";
import {Category} from "../../../models/entities/category";

export interface IAddEditTopicScope extends IScope {

    //#region Properties

    // Topic information.
    topic: Topic;

    // Selectable categories.
    categories: Category[];

    // Add/edit form
    addEditForm: IFormController;

    // Whether preview mode is on or off.
    bIsInPreviewMode: boolean;

    // Whether editor is initialized or not.
    bIsEditorInitialized: boolean;

    //#endregion

    //#region Methods

    // Called when add-edit topic is clicked.
    ngOnTopicSubmitted($event: IAngularEvent): void;

    // Called when preview is toggled.
    ngOnPreviewToggled(): void;

    // Called when cancel is clicked.
    ngOnCancelClicked(): void;

    // Called when component is initialized.
    ngOnInit(): void;

    //#endregion
}