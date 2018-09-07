export class SearchResult<T> {

  // List of records in the search result.
  records: Array<T> = new Array<T>();

  // Total records number.
  total: number = 0;
}
