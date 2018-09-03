import {IScope} from "angular";

export interface IAppScope extends IScope {

    //#region Methods

    // Called when component is initialized.
    ngOnInit(): void;

    //#endregion
}