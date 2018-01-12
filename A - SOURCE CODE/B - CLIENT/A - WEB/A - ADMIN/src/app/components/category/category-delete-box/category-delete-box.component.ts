import {Component, EventEmitter, Input, Output} from '@angular/core';
import * as _ from "lodash";
import {CategoryDetailsViewModel} from "../../../../viewmodels/category/category-details.view-model";
import {CategoryViewModel} from "../../../../viewmodels/category/category.view-model";

@Component({
    selector: 'category-delete-box',
    templateUrl: 'category-delete-box.component.html',
    exportAs: 'category-delete-box'
})

export class CategoryDeleteBoxComponent{

    //#region Properties

  /*
  * Category information.
  * */
  @Input('category')
  private category: CategoryViewModel;

  /*
  * Event which will be raised when confirm button is clicked.
  * */
  @Output('click-confirm')
  private eClickConfirm: EventEmitter<CategoryViewModel>;

  /*
  * Event which will be raised when cancel button is clicked.
  * */
  @Output('click-cancel')
  private eClickCancel: EventEmitter<CategoryViewModel>;

    //#endregion

    //#region Constructor

    // Initiate component with injectors.
    public constructor(){
      this.eClickConfirm = new EventEmitter<CategoryViewModel>();
      this.eClickCancel = new EventEmitter<CategoryViewModel>();
    }
    //#endregion

    //#region Methods

    // Set details and attach to delete box.
    public setDetails(details: CategoryViewModel): void{
        this.category = _.cloneDeep(details);
    }

    // Get category details.
    public getDetails(): CategoryViewModel{
        return this.category;
    }

    //#endregion
}
