import {Inject, Injectable} from "@angular/core";
import {SortDirection} from "../enumerations/sort-direction";
import {CategorySortProperty} from "../enumerations/order/category-sort-property";
import {AccountSortProperty} from "../enumerations/order/account-sort-property";
import {AccountStatus} from "../enumerations/account-status";
import {CommentReportSortProperty} from "../enumerations/order/comment-report-sort-property";
import {TextSearchMode} from "../enumerations/text-search-mode";
import {PostSortProperty} from "../enumerations/order/post-sort-property";
import {PostReportSortProperty} from "../enumerations/order/post-report-sort-property";
import {KeyValuePair} from "../models/key-value-pair";
import {ResponseOptions, Response} from "@angular/http";
import {IApiService} from "../interfaces/services/api/api-service.interface";
import {IConfigurationService} from "../interfaces/services/configuration-service.interface";
import {Dictionary} from '../models/dictionary';

@Injectable()
export class ConfigurationService implements IConfigurationService{


  //#region Properties

  /*
  * List of account statuses (key-value).
  * */
  private accountStatuses: Array<KeyValuePair<AccountStatus>>;

  /*
  * List of directions can be used for sorting.
  * */
  private sortDirections: Array<KeyValuePair<SortDirection>>;

  //#endregion

  // List of page record number which can be selected on the screen.
  public pageRecords: number[];

  // Modes of text search.
  public textSearchModes: Dictionary<TextSearchMode>;

  // List of properties which can be used for accounts sorting.
  public accountSortProperties: Dictionary<AccountSortProperty>;

  // List of properties which can be used for categories sorting.
  public categorySortProperties: Dictionary<CategorySortProperty>;

  // List of properties which are used for sorting.
  public postSortProperties: Dictionary<PostSortProperty>;

  // List of properties which are used for sorting.
  public postReportSortProperties: Dictionary<PostReportSortProperty>;

  // List of properties which can be used for comments sorting.
  public commentReportSortProperties: Dictionary<CommentReportSortProperty>;// List of items which can be selected in accounts list.

  // List of account statuses which can be selected.
  public accountStatusSelections: Dictionary<AccountStatus>;

  // Initiate instance of service with default settings.
  public constructor(@Inject('IApiService') public apiService: IApiService) {

    // Amount of records which can be displayed on the screen.
    this.pageRecords = [5, 10, 15, 20];

    // Initiate list of text search modes.
    this.textSearchModes = this.initiateTextSearchModes();

    // Initiate account sort properties.
    this.accountSortProperties = new Dictionary<AccountSortProperty>();
    this.accountSortProperties.add('Index', AccountSortProperty.index);
    this.accountSortProperties.add('Email', AccountSortProperty.email);
    this.accountSortProperties.add('Nickname', AccountSortProperty.nickname);
    this.accountSortProperties.add('Status', AccountSortProperty.status);
    this.accountSortProperties.add('Joined', AccountSortProperty.joined);
    this.accountSortProperties.add('Last modified', AccountSortProperty.lastModified);

    // Initiate list of account statuses.
    this.accountStatusSelections = this.initializeAccountSelections();

    // Initiate category sort properties.
    this.categorySortProperties = new Dictionary<CategorySortProperty>();
    this.categorySortProperties.add('Index', CategorySortProperty.index);
    this.categorySortProperties.add('Creator', CategorySortProperty.creatorIndex);
    this.categorySortProperties.add('Name', CategorySortProperty.name);
    this.categorySortProperties.add('Created', CategorySortProperty.created);
    this.categorySortProperties.add('Last modified', CategorySortProperty.lastModified);

    // Initiate post sort properties.
    this.postSortProperties = new Dictionary<PostSortProperty>();
    this.postSortProperties.add('Index', PostSortProperty.id);
    this.postSortProperties.add('Owner', PostSortProperty.ownerIndex);
    this.postSortProperties.add('Category', PostSortProperty.categoryIndex);
    this.postSortProperties.add('Created', PostSortProperty.created);

    // Initiate post report sort properties list.
    this.postReportSortProperties = new Dictionary<PostReportSortProperty>();
    this.postReportSortProperties.add('Index', PostReportSortProperty.id);
    this.postReportSortProperties.add('Post', PostReportSortProperty.postIndex);
    this.postReportSortProperties.add('Post owner', PostReportSortProperty.postOwnerIndex);
    this.postReportSortProperties.add('Post reporter', PostReportSortProperty.postReporterIndex);
    this.postReportSortProperties.add('Created', PostReportSortProperty.created);

    // Initiate list of comment report sort properties.
    this.commentReportSortProperties = this.initiateCommentReportSortProperties();
  }

  /*
  * Maximum number of records which can be displayed on page.
  * */
  public getMaxPageRecords(): number {
    return 20;
  }

  /*
  * Get minimum number of records can be displayed per page.
  * */
  public getMinPageRecords(): number {
    return 5;
  }

  /*
  * Initiate list of account statuses selection.
  * */
  private initializeAccountSelections(): Dictionary<AccountStatus> {
    let accountStatusItems = new Dictionary<AccountStatus>();

    accountStatusItems.add('Inactive', AccountStatus.Disabled);
    accountStatusItems.add('Pending', AccountStatus.Pending);
    accountStatusItems.add('Active', AccountStatus.Active);

    return accountStatusItems;
  }

  /*
  * Initiate comment report sort properties list.
  * */
  private initiateCommentReportSortProperties(): Dictionary<CommentReportSortProperty> {

    // Initiate properties list.
    let commentReportSortProperties = new Dictionary<CommentReportSortProperty>();
    commentReportSortProperties.add('Index', CommentReportSortProperty.Index);
    commentReportSortProperties.add('Created', CommentReportSortProperty.Index);

    return commentReportSortProperties;
  }

  /*
  * Initiate text search modes list.
  * */
  private initiateTextSearchModes(): Dictionary<TextSearchMode> {

    // Initiate text search modes.
    let textSearchModes = new Dictionary<TextSearchMode>();
    textSearchModes.add('Equals', TextSearchMode.equals);
    textSearchModes.add('Equals case-insensitively', TextSearchMode.equalsIgnoreCase);
    textSearchModes.add('Contains', TextSearchMode.contains);
    return textSearchModes;
  }

  /*
  * Get list of account statuses (key-value)
  * */
  public getAccountStatuses(): Promise<Response>{
    if (this.accountStatuses) {
      let options = new ResponseOptions();
      options.status = 200;
      options.body = this.accountStatuses;

      // Return promise.
      return Promise.resolve(new Response(options));
    }

    // Find api http instance.
    let http = this.apiService.getInstance();

    // Find promise.
    let pGetAccountStatuses = http.get('/assets/data/account-statuses.json').toPromise();
    pGetAccountStatuses.then((x: Response) => {
      this.accountStatuses = <Array<KeyValuePair<AccountStatus>>> x.json();
    });

    return pGetAccountStatuses;
  }

  /*
  * Get sort directions configured in setting file.
  * */
  public getSortDirections(): Promise<Response> {
    // Settings have been cached.
    if (this.sortDirections){
      let options = new ResponseOptions();
      options.status = 200;
      options.body = this.sortDirections;
      return Promise.resolve(new Response(options));
    }

    let pGetSortDirections = this.apiService.getInstance()
      .get('/assets/data/sort-directions.json')
      .toPromise();

    pGetSortDirections.then((x: Response) => {
      this.sortDirections = <Array<KeyValuePair<SortDirection>>> x.json();
    });

    return pGetSortDirections;
  }

  /*
  * Get key name from value.
  * */
  public getKeyByValue(keyValuePairs: Array<KeyValuePair<any>>, value: any): string{
    let pairs = keyValuePairs.filter((x: KeyValuePair<any>) => {
      return x.value == value;
    });

    if (!pairs || pairs.length < 1)
      return null;

    return pairs[0].key;
}


}
