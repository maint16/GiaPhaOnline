export interface IRecaptchaScope {

    //#region Methods

    /*
    * Called when captcha widget is initialized.
    * */
    ngOnWidgetInitialized(widgetId: string): void;

    //#endregion
}