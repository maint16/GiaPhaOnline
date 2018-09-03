export interface IUiService {

    //#region Methods

    /*
    * Block application UI.
    * */
    blockAppUI(): void;

    /*
    * Unblock application UI.
    * */
    unblockAppUI(): void;

    // Find element.
    getElement(query: string): any;

    //#endregion
}