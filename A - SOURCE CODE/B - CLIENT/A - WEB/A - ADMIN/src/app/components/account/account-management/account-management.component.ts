import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {SearchResult} from "../../../../models/search-result";
import {AccountStatus} from "../../../../enumerations/account-status";
import {AccountSortProperty} from "../../../../enumerations/order/account-sort-property";
import {SortDirection} from "../../../../enumerations/sort-direction";
import {Pagination} from "../../../../models/pagination";
import {ModalDirective} from "ngx-bootstrap";
import {ITimeService} from "../../../../interfaces/services/time-service.interface";
import {ActivatedRoute} from "@angular/router";
import {Sorting} from "../../../../models/sorting";
import {ApplicationSetting} from "../../../../constants/application-setting";
import {SearchUserViewModel} from "../../../../viewmodels/user/search-user.view-model";
import {IUserService} from "../../../../interfaces/services/user-service.interface";

@Component({
  selector: 'account-management',
  templateUrl: 'account-management.component.html'
})

export class AccountManagementComponent implements OnInit {

  //#region Properties

  /*
  * Modal which contains account profile box component.
  * */
  @ViewChild('profileBoxContainer')
  private profileBoxContainer: ModalDirective;

  /*
  * Modal which contains account search component.
  * */
  @ViewChild('accountSearchBoxContainer')
  private accountSearchBoxContainer: ModalDirective;

  /*
  * List of accounts.
  * */
  public searchResult: SearchResult<Account>;

  /*
  * List of conditions to search for accounts.
  * */
  private conditions: SearchUserViewModel;

  /*
  * Whether components are busy or not.
  * */
  private isBusy: boolean;

  /*
  * Point to enumeration to take values out.
  * */
  private AccountStatus = AccountStatus;

  //#endregion

  //#region Constructor

  /*
  * Initiate component with injections.
  * */
  public constructor(@Inject('IUserService') public accountService: IUserService,
                     @Inject("ITimeService") public timeService: ITimeService,
                     public activatedRoute: ActivatedRoute) {

    // Initiate search conditions.
    let sorting = new Sorting<AccountSortProperty>();
    sorting.direction = SortDirection.Ascending;
    sorting.property = AccountSortProperty.index;

    let pagination = new Pagination();

    this.conditions = new SearchUserViewModel();
    this.conditions.sorting = sorting;
    this.conditions.pagination = pagination;

    // Initiate search result.
    this.searchResult = new SearchResult<Account>();
  }

  //#endregion

  //#region Methods

  /*
  * Callback which is fired when search button of category search box is clicked.
  * */
  public clickSearch(conditions: SearchUserViewModel): void {

    // Condition is specified.
    if (conditions)
      this.conditions = conditions;

    // Reset pagination information.
    let pagination: Pagination = this.conditions.pagination;
    if (!pagination) {
      // Reset to page 1.
      pagination = new Pagination();
      pagination.page = 1;
      pagination.records = ApplicationSetting.maxPageRecords;
    }

    this.conditions.pagination = pagination;

    // Make component be busy.
    this.isBusy = true;

    // this.accountService.getUsers(this.conditions)
    //   .then((x: Response) => {
    //
    //     // Find list of accounts which has been found from service.
    //     this.searchResult = <SearchResult<Account>> x.json();
    //
    //     // Cancel loading.
    //     this.isBusy = false;
    //
    //     // Close account search modal dialog.
    //     this.accountSearchBoxContainer.hide();
    //   })
    //   .catch((x: Response) => {
    //
    //     // Cancel loading.
    //     this.isBusy = false;
    //   });
  }

  /*
  * Callback which is fired when change account information button is clicked.
  * */
  public clickChangeAccountInfo(account: Account): void {

    // // Update account information into profile box.
    // this.profileBox.setProfile(account);

    // Display modal.
    this.profileBoxContainer.show();
  }

  /*
  * Callback which is fired when change account information ok button is clicked.
  * */
  public clickConfirmAccountInfo(): void {

    // // Find account from profile box.
    // let account = this.profileBox.getProfile();
    //
    // // Account is invalid.
    // if (account == null) {
    //   return;
    // }
    //
    // // Set components to loading state.
    // this.isBusy = true;
    //
    // // Send request to service to change account information.
    // this.accountService.editUserProfile(account.id, account)
    //   .then((response: Response) => {
    //
    //     // Cancel loading.
    //     this.isBusy = false;
    //
    //     // Close the dialog.
    //     this.profileBoxContainer.hide();
    //
    //     // Reload the page.
    //     this.clickSearch(null);
    //   })
    //   .catch((response: Response) => {
    //
    //     // Cancel loading process.
    //     this.isBusy = false;
    //
    //     // Close the dialog.
    //     this.profileBoxContainer.hide();
    //   });
  }

  /*
  * Called when component has been successfully rendered.
  * */
  public ngOnInit(): void {

    // Components are not busy loading.
    this.isBusy = false;

    // Load all accounts from service.
    this.clickSearch(null);
  }

  /*
  * Callback which is called when page is clicked on.
  * */
  public clickChangePage($event: any): void {

    // Event is invalid.
    if (!$event)
      return;

    // Find page from event.
    let iPage = $event.page;
    if (!iPage)
      iPage = 1;

    let pagination = this.conditions.pagination;
    pagination.page = iPage;
    this.conditions.pagination = pagination;

    // Update page.
    this.conditions.pagination.page = iPage;

    // Base on specific condition to load accounts list from database.
    this.clickSearch(null);
  }

  //#endregion
}
