import {Component, EventEmitter, Inject, Input, OnInit, Output} from '@angular/core';
import {Response} from "@angular/http";
import {SearchCategoriesViewModel} from "../../../../viewmodels/category/search-categories.view-model";
import {CategorySortProperty} from "../../../../enumerations/order/category-sort-property";
import {SearchAccountsViewModel} from "../../../../viewmodels/accounts/search-accounts.view-model";
import {TextSearch} from "../../../../models/text-search";
import {TextSearchMode} from "../../../../enumerations/text-search-mode";
import {Pagination} from "../../../../models/pagination";
import {SearchResult} from "../../../../models/search-result";
import {Account} from '../../../../models/entities/account';
import {IAccountService} from "../../../../interfaces/services/api/account-service.interface";
import {KeyValuePair} from "../../../../models/key-value-pair";
import {SortDirection} from "../../../../enumerations/sort-direction";
import {IConfigurationService} from "../../../../interfaces/services/configuration-service.interface";
import {IDictionary} from "../../../../interfaces/dictionary.interface";
import {Dictionary} from '../../../../models/dictionary';

@Component({
  selector: 'category-search-box',
  templateUrl: 'category-search-box.component.html'
})

export class CategorySearchBoxComponent implements OnInit {

  //#region Properties

  /*
  * List of sort directions.
  * */
  private sortDirections: Array<KeyValuePair<SortDirection>>;

  /*
  * Collection of conditions which are used for searching categories.
  * */
  @Input('conditions')
  private conditions: SearchCategoriesViewModel;

  /*
  * Whether component is busy or not.
  * */
  @Input('is-busy')
  private bIsBusy: boolean;

  /*
  * Event which is fired when modal button is closed.
  * */
  @Output('close')
  private eCloseModal: EventEmitter<any>;

  /*
  * Event which is fired when search button is clicked.
  * */
  @Output('search')
  private eSearch: EventEmitter<SearchCategoriesViewModel>;
  /*
  * List of accounts which are used for typeahead binding.
  * */
  private accounts: Array<Account>;

  /*
  * List of properties which can be used for sorting categories.
  * */
  private categorySortProperties: IDictionary<CategorySortProperty>;

  //#endregion

  //#region Constructor

  /*
  * Initiate component with default dependencies injection.
  * */
  public constructor(@Inject('IAccountService') public clientAccountService: IAccountService,
                     @Inject('IConfigurationService') public configurationService: IConfigurationService) {
    // Initiate account typeahead data.
    this.accounts = new Array<Account>();

    // Initiate category sorting properties.
    this.categorySortProperties = new Dictionary<CategorySortProperty>();
    this.categorySortProperties.add('Index', CategorySortProperty.index);
    this.categorySortProperties.add('Creator', CategorySortProperty.creatorIndex);
    this.categorySortProperties.add('Category name', CategorySortProperty.name);
    this.categorySortProperties.add('Created', CategorySortProperty.created);
    this.categorySortProperties.add('Last modified', CategorySortProperty.lastModified);

    // Event emitters initialization.
    this.eCloseModal = new EventEmitter<any>();
    this.eSearch = new EventEmitter<SearchCategoriesViewModel>();
  }

  //#endregion

  //#region Methods

  /*
  * Callback which is fired when control is starting to load data of accounts from service.
  * */
  public loadAccounts(keyword: string): void {

    // Initiate find account conditions.
    let conditions = new SearchAccountsViewModel();

    // Update account which should be searched for.
    if (conditions.email == null)
      conditions.email = new TextSearch();

    conditions.email.value = keyword;
    conditions.email.mode = TextSearchMode.contains;

    // Initiate pagination.
    let pagination = new Pagination();

    // Pagination update.
    conditions.pagination = pagination;

    // Find accounts with specific conditions.
    this.clientAccountService.getAccounts(conditions)
      .then((x: Response | any) => {
        // Analyze find account response view model.
        let result = <SearchResult<Account>> x.json();

        // Find list of accounts which has been responded from service.
        this.accounts = result.records;
      })
      .catch((x: Response) => {
      });
  }

  /*
  * Update accounts list.
  * */
  public updateAccounts(accounts: Array<Account>): void {
    let ids = accounts.map((x: Account) => {
      return x.id;
    });
    if (ids != null && ids.length > 0) {
      this.conditions.creatorIndex = ids[0];
    } else {
      this.conditions.creatorIndex = null;
    }
  }

  /*
  * Callback which is fired when component has been loaded.
  * */
  public ngOnInit(): void {
    // Load all accounts.
    this.loadAccounts('');

    // Load sort directions.
    this.configurationService.getSortDirections()
      .then((x: Response) => {
        this.sortDirections = <Array<KeyValuePair<SortDirection>>> x.json();
      });
  }

  //#endregion
}
