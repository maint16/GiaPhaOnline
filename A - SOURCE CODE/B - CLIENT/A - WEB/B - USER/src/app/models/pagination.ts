export class Pagination {

    //#region Properties

    // Result page.
    public page: number;

    // Number of records per page.
    public records: number;

    //#endregion

    //#region Constructor

    /*
    * Initialize model with default settings.
    * */
    public constructor(){
        this.page = 1;
        this.records = 1;
    }

    //#endregion
}