import {Component, Inject, Input, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {ICategoryService} from '../../../interfaces/services/category-service.interface';
import {SearchResult} from '../../../models/search-result';
import {Pagination} from '../../../models/pagination';
import {Category} from '../../../models/entities/category';
import {LoadCategoryViewModel} from '../../../view-models/category/load-category.view-model';
import {LazyLoadEvent} from 'primeng/api';
import {Router} from '@angular/router';
import {TranslateService} from '@ngx-translate/core';
import {CategoryStatus} from '../../../enums/category-status.enum';

@Component({
  selector: 'manage-category',
  templateUrl: 'manage-category.component.html',
  styleUrls: ['manage-category.component.css']
})

export class ManageCategoryComponent implements OnInit {
  public loadCategoryCondition: LoadCategoryViewModel;
  public availableCGStatuses = [null, CategoryStatus.active, CategoryStatus.disabled];
  public pagination: Pagination;
  public searchName: string;
  public cgStatus: any;
  public searchCategoryResults: SearchResult<Category>;

  //#region Constructor

  public constructor(@Inject('ICategoryService') private categoryService: ICategoryService, private toastr: ToastrService,
                     public route: Router, private translate: TranslateService) {
    translate.setDefaultLang('en');
    this.searchCategoryResults = new SearchResult<Category>();
  }

  //#endregion

  //#region Methods

  public ngOnInit(): void {
    // Set default value for status dropdown list
    this.cgStatus = 'null';
    // Set paging
    this.loadCategoryCondition = new LoadCategoryViewModel();
    this.pagination = new Pagination();
    this.pagination.page = 1;
    this.pagination.records = 5;
    this.loadCategoryCondition.pagination = this.pagination;
    // Load CG using specific conditions on first load
    this.categoryService.loadCategory(this.loadCategoryCondition)
      .subscribe((loadCategorysResult: SearchResult<Category>) => {
        this.searchCategoryResults.records = loadCategorysResult.records;
        this.searchCategoryResults.total = loadCategorysResult.total;
      });
  }

  // Load CG when change page
  public loadCategoryLazy(event: LazyLoadEvent) {
    if (this.searchCategoryResults.records) {
      this.pagination = new Pagination();
      this.pagination.page = event.first / event.rows + 1;
      this.pagination.records = event.rows;
      this.loadCategoryCondition.pagination = this.pagination;
      this.categoryService.loadCategory(this.loadCategoryCondition)
        .subscribe((loadCategorysResult: SearchResult<Category>) => {
          this.searchCategoryResults.records = loadCategorysResult.records;
          this.searchCategoryResults.total = loadCategorysResult.total;
        });
    }
  }

  // Route to add new CG component
  public addNewCategory(): void {
    this.route.navigate(['category/add-new']);
  }

  // Route to edit cg component
  public editCategory(id) {
    this.route.navigate(['category/' + id]);
  }

// Search Category
  public searchCategory() {
    this.pagination = new Pagination();
    this.pagination.page = 1;
    this.pagination.records = 5;
    this.loadCategoryCondition.pagination = this.pagination;
    if (this.cgStatus !== 'null' && this.cgStatus != null) {
      this.loadCategoryCondition.statuses = [+this.cgStatus];
    }
    if (this.searchName != null && this.searchName != undefined) {
      this.loadCategoryCondition.name = this.searchName;
    }
    this.categoryService.loadCategory(this.loadCategoryCondition)
      .subscribe((loadCategorysResult: SearchResult<Category>) => {
        this.searchCategoryResults.records = loadCategorysResult.records;
        this.searchCategoryResults.total = loadCategorysResult.total;
      });

    //#endregion
  }
}
