import {Component, EventEmitter, Input, OnInit, Inject, Output} from '@angular/core';
import {AccountSortProperty} from "../../../../enumerations/order/account-sort-property";
import {AccountStatus} from "../../../../enumerations/account-status";
import {KeyValuePair} from "../../../../models/key-value-pair";
import {SortDirection} from "../../../../enumerations/sort-direction";
import {IDictionary} from "../../../../interfaces/dictionary.interface";
import {Dictionary} from '../../../../models/dictionary';
import {SearchUserViewModel} from "../../../../viewmodels/user/search-user.view-model";
import {IUserService} from "../../../../interfaces/services/user-service.interface";

@Component({
  selector: 'account-search-box',
  templateUrl: 'account-search-box.component.html',
  exportAs: 'account-search-box'
})

export class AccountSearchBoxComponent implements OnInit {

  //#region Properties

  /*
  * Whether records are being loaded from server or not.
  * */
  @Input('is-loading')
  public isLoading: boolean;

  /*
  * Collection of conditions which are used for searching categories.
  * */
  @Input('conditions')
  private conditions: SearchUserViewModel;

  /*
  * List of accounts which can be selected.
  * */
  public accounts: Array<Account>;

  /*
  * List of properties which can be used for sorting.
  * */
  private sortProperties: IDictionary<AccountSortProperty>;

  /*
  * List of account statuses.
  * */
  private accountStatuses: Array<KeyValuePair<AccountStatus>>;

  /*
  * List of sort directions.
  * */
  private sortDirections: Array<KeyValuePair<SortDirection>>;

  /*
  * Event emitter which should be raised when search button is clicked.
  * */
  @Output('search')
  private eSearch: EventEmitter<SearchUserViewModel>;

  /*
  * Event emitter which should be raised when close button is clicked.
  * */
  @Output('close')
  private eCloseModal: EventEmitter<void>;

  //#endregion

  //#region Constructor

  // Initiate component with default dependency injection.
  public constructor(@Inject('IUserService') public accountService: IUserService) {

    // Initiate event emitters.
    this.accounts = new Array<Account>();

    // Initiate list of properties which can be used for sorting.
    let sortProperties = new Dictionary<AccountSortProperty>();
    sortProperties.add('Index', AccountSortProperty.index);
    sortProperties.add('Email', AccountSortProperty.email);
    sortProperties.add('Nickname', AccountSortProperty.nickname);
    sortProperties.add('Status', AccountSortProperty.status);
    sortProperties.add('Joined', AccountSortProperty.joined);
    sortProperties.add('Last modified', AccountSortProperty.lastModified);

    this.sortProperties = sortProperties;

    this.eSearch = new EventEmitter<SearchUserViewModel>();
    this.eCloseModal = new EventEmitter<void>();
  }

  //#endregion

  //#region Methods

  /*
  * Callback which is fired when status button is toggled.
  * */
  public toggleStatuses(status: AccountStatus): void {
    //
    // // Statuses list hasn't been initialized.
    // if (this.conditions.statuses == null) {
    //   this.conditions.statuses = new Array<AccountStatus>();
    //   this.conditions.statuses.push(status);
    //   return;
    // }
    //
    // // Find status in the list.
    // let index = this.conditions.statuses.indexOf(status);
    // if (index == -1) {
    //   this.conditions.statuses.push(status);
    //   return;
    // }
    //
    // this.conditions.statuses.splice(index, 1);
  }

  // Callback which is fired when statuses selection is updated.
  public updateStatuses(statuses: Array<KeyValuePair<AccountStatus>>): void {
    // this.conditions.statuses = statuses.map((x: KeyValuePair<AccountStatus>) => {
    //   return x.value
    // });
  }

  /*
  * Callback which is fired when component has been loaded successfully.
  * */
  public ngOnInit(): void {

    // // Load account statuses list.
    // this.configurationService.getAccountStatuses()
    //   .then((x: Response) => {
    //     this.accountStatuses = <Array<KeyValuePair<AccountStatus>>> x.json();
    //   });
    //
    // // Load sort directions.
    // this.configurationService.getSortDirections()
    //   .then((x: Response) => {
    //     this.sortDirections = <Array<KeyValuePair<SortDirection>>> x.json();
    //   })
  }

  //#endregion
}
