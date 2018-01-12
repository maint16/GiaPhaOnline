import {Component, Inject, OnInit, ViewChild} from '@angular/core'
import {Response} from "@angular/http";
import {CategoryInitiateBoxComponent} from "./category-initiator-box/category-initiate-box.component";
import {CategoryDetailBoxComponent} from "./category-detail-box/category-detail-box.component";
import {CategoryDeleteBoxComponent} from "./category-delete-box/category-delete-box.component";
import {SearchCategoriesViewModel} from "../../../viewmodels/category/search-categories.view-model";
import {Sorting} from "../../../models/sorting";
import {SortDirection} from "../../../enumerations/sort-direction";
import {CategorySortProperty} from "../../../enumerations/order/category-sort-property";
import {Pagination} from "../../../models/pagination";
import {SearchResult} from "../../../models/search-result";
import {CategoryDetailsViewModel} from "../../../viewmodels/category/category-details.view-model";
import {Category} from "../../../models/entities/category";
import {ModalDirective} from "ngx-bootstrap";
import {ICategoryService} from "../../../interfaces/services/api/category-service.interface";
import {ToastrService} from "ngx-toastr";
import {ITimeService} from "../../../interfaces/services/time-service.interface";
import {IConfigurationService} from "../../../interfaces/services/configuration-service.interface";
import {CategoryViewModel} from "../../../viewmodels/category/category.view-model";
import {InitiateCategoryViewModel} from "../../../viewmodels/category/initiate-category.view-model";
import {ActivatedRoute, Route, Router} from "@angular/router";

@Component({
  selector: 'category-management',
  templateUrl: 'category-management.component.html'
})

export class CategoryManagementComponent implements OnInit {

  //#region Properties

  // Modal which contains category initiator.
  @ViewChild('categoryInitiatorModal')
  public initiateCategoryInfoModal: ModalDirective;

  // Editor contains fields to edit category.
  @ViewChild('categoryDetailBox')
  public categoryDetailBox: CategoryDetailBoxComponent;

  // Modal contains category search box.
  @ViewChild('categorySearchModal')
  public categorySearchModal: ModalDirective;

  // List of categories responded from service.
  private searchResult: SearchResult<CategoryViewModel>;

  // Whether records are being searched or not.
  private bIsBusy: boolean;

  // Conditions which are used for searching categories.
  private conditions: SearchCategoriesViewModel;

  // List of accouts which are used for selecting.
  private accounts: Array<Account>;

  //#endregion

  //#region Constructor

  // Initiate component with dependency injections.
  public constructor(public toastr: ToastrService,
                     @Inject('ITimeService') public timeService: ITimeService,
                     @Inject('IConfigurationService') public configurationService: IConfigurationService,
                     @Inject('ICategoryService') public categoryService: ICategoryService,
                     public activatedRoute: ActivatedRoute) {

    // Initiate search condition.
    let conditions = new SearchCategoriesViewModel();
    let sorting = new Sorting<CategorySortProperty>();
    sorting.direction = SortDirection.Ascending;
    sorting.property = CategorySortProperty.index;
    conditions.sorting = sorting;

    let pagination = new Pagination();
    conditions.pagination = pagination;

    // Update initial conditions.
    this.conditions = conditions;

    // Initiate categories search result.
    this.searchResult = new SearchResult<CategoryViewModel>();
  }

  //#endregion

  //#region Methods

  // This callback is called when user confirms to delete the selected category.
  public deleteCategory(category: CategoryViewModel) {

    // Find category by using specific conditions.
    if (!category)
      return;

    // Make the loading start.
    this.bIsBusy = true;

    // Initiate search category view model.
    let conditions = new SearchCategoriesViewModel();
    conditions.id = category.id;

    // Call category service to delete the selected category.
    this.categoryService.deleteCategories(conditions)
      .then((x: Response | any) => {
        // Reload the categories list.
        this.clickSearch();
      })
      .catch((response: any) => {
        // Cancel loading.
        this.bIsBusy = false;
      });
  }

  // Callback which is fired when change category detail action is confirmed.
  public editCategoryProfile(profile: CategoryViewModel) {

    if (!profile)
      return;

    // Initiate category information.
    let information = new Category();
    information.id = profile.id;
    information.name = profile.name;

    // Start loading.
    this.bIsBusy = true;

    // Call service to update category information.
    this.categoryService.editCategoryDetails(profile.id, information)
      .then((x: Response | any) => {
        // Clear busy state.
        this.bIsBusy = false;

        // Reload the categories list.
        this.clickSearch();
      })
      .catch((response: Response | any) => {
        // Cancel loading.
        this.bIsBusy = false;
      });

  }

  // This callback is fired when confirm button is clicked which tells client to send request to service to create a new category.
  public initiateCategory(information: InitiateCategoryViewModel): void {

    if (!information)
      return;

    // Lock the component.
    this.bIsBusy = true;

    // Call service to initiate category.
    this.categoryService.initiateCategory(information)
      .then((x: Response | any) => {
        // Parse information of response.
        let category = <Category> x.json();

        // Display notification to client screen.
        this.toastr.success(`${category.name} has been created successfully`, 'Thông tin hệ thống');

        // Close the modal.
        this.initiateCategoryInfoModal.hide();

        // Reload search results.
        this.clickSearch();
      })
      .catch((response: Response | any) => {
        // Cancel content loading.
        this.bIsBusy = false;
      });
  }

  /*
  * Callback which is fired when search button of category search box is clicked.
  * */
  public clickSearch(): void {

    // Freeze the find box.
    this.bIsBusy = true;

    // Find categories by using specific conditions.
    this.categoryService.getCategories(this.conditions)
      .then((x: Response) => {

        // Update categories list.
        this.searchResult = <SearchResult<CategoryViewModel>> x.json();

        // Unfreeze the category find box.
        this.bIsBusy = false;
      })
      .catch((response: Response | any) => {
        // Unlock screen components.
        this.bIsBusy = false;

        // Hide the category search modal.
        if (this.categorySearchModal)
          this.categorySearchModal.hide();
      });
  }

  // Callback which is fired when page selection is changed.
  public selectPage(page: number): void {
    // Update pagination page.
    this.conditions.pagination.page = page;

    // Call search function.
    this.clickSearch();
  }

  // This callback is fired when category management component is initiated.
  public ngOnInit() {
    // Search for categories using specific conditions.
    this.clickSearch();
  }

  //#endregion
}
