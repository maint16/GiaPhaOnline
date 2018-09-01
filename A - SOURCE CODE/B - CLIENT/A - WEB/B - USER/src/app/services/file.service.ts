import {IFileService} from "../interfaces/services/file-service.interface";
import {IPromise, IQService} from "angular";

export class FileService implements IFileService{

    //#region Constructor

    //#endregion

    //#region Methods

    // Convert blob to base-64 encoded file.
    public toEncodedFile(blob: any): IPromise<string | ArrayBuffer> {
        return this.$q((resolve) => {
            let fileReader = new FileReader();
            fileReader.readAsDataURL(blob);
            fileReader.onloadend = () => {
                resolve(fileReader.result);
            }
        });

    }

    //#endregion

    //#region Constructor

    // Initialize service with injectors.
    public constructor(public $q: IQService){

    }

    //#endregion

}