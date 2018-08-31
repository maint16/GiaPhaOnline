import {IScope} from "angular";
import {Topic} from "../../../models/entities/topic";

export interface IAddEditTopicScope extends IScope {

    //#region Properties

    // Topic information.
    topic: Topic;

    //#endregion

    //#region Methods

    /*
    * Called when add-edit topic is clicked.
    * */
    ngOnAddEditTopicClicked($event): void;

    //#endregion
}