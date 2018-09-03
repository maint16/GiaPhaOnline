import {IUiService} from "../interfaces/services/ui-service.interface";

/* @ngInject */
export class UiService implements IUiService {

    //#region Constructor

    // Initialize service with injectors.
    public constructor(public blockUI: any){

    }

    //#endregion

    //#region Methods

    // Add loading screen to app UI.
    public blockAppUI(): void {
        const appBlockUI = this.blockUI.instances.get('appBlockUI');
        if (!appBlockUI)
            return;

        appBlockUI.start();
    }

    // Delete loading screen from app UI.
    public unblockAppUI(): void {
        const appBlockUI = this.blockUI.instances.get('appBlockUI');
        if (!appBlockUI)
            return;

        appBlockUI.stop();
    }

    // Find element.
    public getElement(query: string): any {
        return $(query);
    }

    //#endregion
}