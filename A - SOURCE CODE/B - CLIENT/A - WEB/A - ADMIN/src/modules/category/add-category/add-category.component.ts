import {Component, Inject, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {Pagination} from '../../../models/pagination';
import {ActivatedRoute, Router} from '@angular/router';
import {SearchResult} from '../../../models/search-result';
import {TranslateService} from '@ngx-translate/core';
import {AddCategoryViewModel} from '../../../view-models/category/add-category.view-model';
import {LoadCategoryViewModel} from '../../../view-models/category/load-category.view-model';
import {Category} from '../../../models/entities/category';
import {CategoryStatus} from '../../../enums/category-status.enum';
import {ICategoryService} from '../../../interfaces/services/category-service.interface';

@Component({
  selector: 'add-category',
  templateUrl: 'add-category.component.html',
  styleUrls: ['add-category.component.css']
})

export class AddCategoryComponent implements OnInit {
  public loadCategoryCondition: LoadCategoryViewModel;
  public category: AddCategoryViewModel;
  public pagination: Pagination;
  public isEditMode: boolean;
  public itemId: number;
  public availableCGStatuses = [CategoryStatus.active, CategoryStatus.disabled];

  //#region Constructor
  public constructor(@Inject('ICategoryService') private categoryService: ICategoryService, private toastr: ToastrService,
                     public router: Router, private route: ActivatedRoute, private translate: TranslateService) {
    translate.setDefaultLang('en');
    this.category = new AddCategoryViewModel();
    // check if route has parameter => in edit mode => load model from database
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.itemId = params['id'];
        this.isEditMode = true;
        this.loadCategoryCondition = new LoadCategoryViewModel();
        this.loadCategoryCondition.id = [this.itemId];
        this.categoryService.loadCategory(this.loadCategoryCondition)
          .subscribe((loadCategorysResult: SearchResult<Category>) => {
            this.category.name = loadCategorysResult.records[0].name;
            this.category.description = loadCategorysResult.records[0].description;
            this.category.status = loadCategorysResult.records[0].status;
          });
      } else {
        this.isEditMode = false;
      }
    });
  }

  //#endregion
  //#region methods
  public saveCategory($event) {
    // if is in edit mode
    if (this.isEditMode) {
      this.categoryService.updateCategory(this.itemId, this.category).subscribe((data: any) => {
        this.toastr.success('Update Category Successfully');
        // Redirect to manage-category.
        this.router.navigate(['/category/manage']);
      });
    }
    // if is in create new mode
    else {
      this.categoryService.addCategory(this.category).subscribe((data: any) => {
        this.toastr.success('Save Category Successfully');
        // Redirect to manage-category.
        this.router.navigate(['/category/manage']);
      });
    }

  }

  public ngOnInit() {
    this.category = new AddCategoryViewModel();
  }

// route to manage CG component when click cancel
  public cancel() {
    this.router.navigate(['/category/manage']);
  }
  // #endregion
}
