export class SearchResult<T> {

    //#region Properties

    // Records that will be displayed on the screen.
    public records: Array<T>;

    // Total records number that match search condition.
    public total: number;

    //#endregion

    //#region Constructor

    public constructor(){
        this.records = [];
        this.total = 0;
    }

    //#endregion
}