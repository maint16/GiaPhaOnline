import {Component, Inject, Input, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {ICategoryGroupService} from '../../../interfaces/services/category-group-service.interface';
import {SearchResult} from '../../../models/search-result';
import {Pagination} from '../../../models/pagination';
import {CategoryGroup} from '../../../models/entities/category-group';
import {LoadCategoryGroupViewModel} from '../../../view-models/category-group/load-category-group.view-model';
import {LazyLoadEvent} from 'primeng/api';
import {Router} from '@angular/router';
import {TranslateService} from '@ngx-translate/core';
import {CategoryGroupStatus} from '../../../enums/category-group-status.enum';
import {ISharedEventService} from '../../../interfaces/services/shared-event-service.interface';
import {RealTimeChannelConstant} from '../../../constants/real-time/real-time-channel.constant';
import {RealTimeEventConstant} from '../../../constants/real-time/real-time-event.constant';

@Component({
  selector: 'manage-category-group',
  templateUrl: 'manage-category-group.component.html',
  styleUrls: ['manage-category-group.component.css']
})

export class ManageCategoryGroupComponent implements OnInit {

  public loadCategoryGroupCondition: LoadCategoryGroupViewModel;
  public availableCGStatuses = [null, CategoryGroupStatus.active, CategoryGroupStatus.disabled];
  public pagination: Pagination;
  public searchName: string;
  public cgStatus: any;
  public searchCategoryGroupResults: SearchResult<CategoryGroup>;

  //#region Constructor

  public constructor(@Inject('ICategoryGroupService') private categoryGroupService: ICategoryGroupService,
                     private toastr: ToastrService,
                     public route: Router, private translate: TranslateService,
                     @Inject('ISharedEventService') public sharedEventService: ISharedEventService) {
    translate.setDefaultLang('en');
    this.searchCategoryGroupResults = new SearchResult<CategoryGroup>();

    this.sharedEventService
      .hookMessageChannel(RealTimeChannelConstant.notification, RealTimeEventConstant.addCategoryGroup)
      .subscribe(message => {
        this.handleAddCategoryGroupRealTimeEvent(message);
      });

  }

  //#endregion

  //#region Methods

  public ngOnInit(): void {
    // Set default value for status dropdown list
    this.cgStatus = 'null';
    // Set paging
    this.loadCategoryGroupCondition = new LoadCategoryGroupViewModel();
    this.pagination = new Pagination();
    this.pagination.page = 1;
    this.pagination.records = 5;
    this.loadCategoryGroupCondition.pagination = this.pagination;
    // Load CG using specific conditions on first load
    this.categoryGroupService.loadCategoryGroup(this.loadCategoryGroupCondition)
      .subscribe((loadCategoryGroupsResult: SearchResult<CategoryGroup>) => {
        this.searchCategoryGroupResults.records = loadCategoryGroupsResult.records;
        this.searchCategoryGroupResults.total = loadCategoryGroupsResult.total;
      });
  }

  // Load CG when change page
  public loadCategoryGroupLazy(event: LazyLoadEvent) {
    if (this.searchCategoryGroupResults.records) {
      this.pagination = new Pagination();
      this.pagination.page = event.first / event.rows + 1;
      this.pagination.records = event.rows;
      this.loadCategoryGroupCondition.pagination = this.pagination;
      this.categoryGroupService.loadCategoryGroup(this.loadCategoryGroupCondition)
        .subscribe((loadCategoryGroupsResult: SearchResult<CategoryGroup>) => {
          this.searchCategoryGroupResults.records = loadCategoryGroupsResult.records;
          this.searchCategoryGroupResults.total = loadCategoryGroupsResult.total;
        });
    }
  }

  // Route to add new CG component
  public addNewCategoryGroup(): void {
    this.route.navigate(['category-group/add-new']);
  }

  // Route to edit cg component
  public editCategoryGroup(id) {
    this.route.navigate(['category-group/' + id]);
  }

  // Search Category Group
  public searchCategoryGroup() {
    this.pagination = new Pagination();
    this.pagination.page = 1;
    this.pagination.records = 5;
    this.loadCategoryGroupCondition.pagination = this.pagination;
    if (this.cgStatus !== 'null' && this.cgStatus != null) {
      this.loadCategoryGroupCondition.statuses = [+this.cgStatus];
    }
    if (this.searchName != null && this.searchName != undefined) {
      this.loadCategoryGroupCondition.names = [this.searchName];
    }
    this.categoryGroupService.loadCategoryGroup(this.loadCategoryGroupCondition)
      .subscribe((loadCategoryGroupsResult: SearchResult<CategoryGroup>) => {
        this.searchCategoryGroupResults.records = loadCategoryGroupsResult.records;
        this.searchCategoryGroupResults.total = loadCategoryGroupsResult.total;
      });

    //#endregion
  }

  //#region Event handler


  public handleAddCategoryGroupRealTimeEvent(message: any): void {
    console.log(message);
  }

  //#endregion
}
