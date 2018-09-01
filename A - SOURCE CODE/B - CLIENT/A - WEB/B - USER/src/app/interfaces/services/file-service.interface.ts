import {IPromise} from "angular";

export interface IFileService {

    //#region Methods

    // Convert blob to base-64 encoded file.
    toEncodedFile(blob: any): IPromise<string | ArrayBuffer>;

    //#endregion

    //#region Constructor


    //#endregion
}