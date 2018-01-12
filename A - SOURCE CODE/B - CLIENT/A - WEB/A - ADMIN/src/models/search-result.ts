// This class contains search results which obtained from web api.
export class SearchResult<T> {

  //#region Properties

  /*
  * List of records.
  * */
  public records: Array<T>;

  /*
  * Total condition matched records.
  * */
  public total: number;

  //#endregion
}
