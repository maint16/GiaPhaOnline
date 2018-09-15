import {Component, Inject, Input, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {ICategoryGroupService} from '../../../interfaces/services/category-group-service.interface';
import {SearchResult} from '../../../models/search-result';
import {Pagination} from '../../../models/pagination';
import {CategoryGroup} from '../../../models/entities/category-group';
import {LoadCategoryGroupViewModel} from '../../../view-models/category-group/load-category-group.view-model';
import {LazyLoadEvent} from 'primeng/api';
import {User} from '../../../models/entities/user';
import {Router} from '@angular/router';
import {TranslateService} from '@ngx-translate/core';

@Component({
  selector: 'manage-category-group',
  templateUrl: 'manage-category-group.component.html',
  styleUrls: ['manage-category-group.component.css']
})

export class ManageCategoryGroupComponent implements OnInit {
  public categoryGroups: CategoryGroup[];
  public totalCategoryGroup: number;
  public loadCategoryGroupCondition: LoadCategoryGroupViewModel;
  public pagination: Pagination;
  public constructor(@Inject('ICategoryGroupService') private categoryGroupService: ICategoryGroupService, private toastr: ToastrService,
                     public route: Router, private translate: TranslateService) {
    translate.setDefaultLang('en');
  }
  ngOnInit() {
    this.loadCategoryGroupCondition = new LoadCategoryGroupViewModel();
    this.pagination = new Pagination();
    this.pagination.page = 1;
    this.pagination.records = 5;
    this.loadCategoryGroupCondition.pagination = this.pagination;
    // Load CG using specific conditions on first load
    this.categoryGroupService.loadCategoryGroup(this.loadCategoryGroupCondition)
      .subscribe((loadCategoryGroupsResult: SearchResult<CategoryGroup>) => {
        this.categoryGroups = loadCategoryGroupsResult.records;
        this.totalCategoryGroup = loadCategoryGroupsResult.total;
      });
  }
  // load CG when change page
  loadCGLazy(event: LazyLoadEvent) {
    if (this.categoryGroups) {
      this.pagination = new Pagination();
      this.pagination.page = event.first / event.rows + 1;
      this.pagination.records = event.rows;
      this.categoryGroupService.loadCategoryGroup(this.loadCategoryGroupCondition)
        .subscribe((loadCategoryGroupsResult: SearchResult<CategoryGroup>) => {
          this.categoryGroups = loadCategoryGroupsResult.records;
          this.totalCategoryGroup = loadCategoryGroupsResult.total;
        });
    }
  }
  // route to add new CG component
  addNewCG() {
this.route.navigate(['category-group/add-new']);
  }
  // route to edit cg component
  editCategoryGroup(id) {
    this.route.navigate(['category-group/' + id]);
  }
}
