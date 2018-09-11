import {Component, Inject, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {ICategoryGroupService} from '../../../interfaces/services/category-group-service.interface';
import {Pagination} from '../../../models/pagination';
import {AddCategoryGroup} from '../../../view-models/category-group/add-category-group.view-model';
import {ActivatedRoute, Router} from '@angular/router';
import {CategoryGroup} from '../../../models/entities/category-group';
import {SearchResult} from '../../../models/search-result';
import {LoadCategoryGroupViewModel} from '../../../view-models/category-group/load-category-group.view-model';
import {TranslateService} from '@ngx-translate/core';

@Component({
  selector: 'add-category-group',
  templateUrl: 'add-category-group.component.html',
  styleUrls: ['add-category-group.component.css']
})

export class AddCategoryGroupComponent implements OnInit {
  public loadCategoryGroupCondition: LoadCategoryGroupViewModel;
  public categoryGroup: AddCategoryGroup;
  public pagination: Pagination;
  public isEditMode: boolean;
  public itemId: number;
  public statuss = [
    {category: 'Active', value: 1},
    {category: 'Inactive', value: 2}
  ];
  public constructor(@Inject('ICategoryGroupService') private categoryGroupService: ICategoryGroupService, private toastr: ToastrService,
                     public router: Router, private route: ActivatedRoute, private translate: TranslateService) {
    translate.setDefaultLang('en');
    this.categoryGroup = new AddCategoryGroup();
    // check if route has parameter => in edit mode => load model from database
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.itemId = params['id'];
        this.isEditMode = true;
        this.loadCategoryGroupCondition = new LoadCategoryGroupViewModel();
        this.loadCategoryGroupCondition.id = [this.itemId];
        this.categoryGroupService.loadCategoryGroup(this.loadCategoryGroupCondition)
          .subscribe((loadCategoryGroupsResult: SearchResult<CategoryGroup>) => {
            this.categoryGroup.name = loadCategoryGroupsResult.records[0].name;
            this.categoryGroup.description = loadCategoryGroupsResult.records[0].description;
            this.categoryGroup.status = loadCategoryGroupsResult.records[0].status;
          });
      }else{
        this.isEditMode = false;
      }
    });
  }
  public saveCategoryGroup($event)
  {
    // if is in edit mode
    if(this.isEditMode){
      this.categoryGroupService.updateCategoryGroup(this.itemId, this.categoryGroup).subscribe((data: any)=>{
        this.toastr.success('Update Category Group Successfully');
        // Redirect to manage-category-group.
        this.router.navigate(['/manage-category-group']);
      });
    }
    // if is in create new mode
    else{
      this.categoryGroupService.addCategoryGroup(this.categoryGroup).subscribe((data: any)=>{
        this.toastr.success('Save Category Group Successfully');
        // Redirect to manage-category-group.
        this.router.navigate(['/manage-category-group']);
      });
    }

  }
  ngOnInit() {
    this.categoryGroup = new AddCategoryGroup();
  }
  cancel(){
    this.router.navigate(['/manage-category-group']);
  }
}
