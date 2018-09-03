import {ILocalStorageServiceProvider} from "angular-local-storage";

export class LocalStorageConfiguration {

    //#region Constructor

    public constructor(localStorageProvider: ILocalStorageServiceProvider){
        localStorageProvider.setPrefix('i-confess');
    }

    //#endregion
}