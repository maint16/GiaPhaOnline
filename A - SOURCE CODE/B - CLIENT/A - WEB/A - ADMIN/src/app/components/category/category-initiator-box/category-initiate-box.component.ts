import {Component, EventEmitter, Input, Output, ViewChild} from '@angular/core';
import {NgForm} from "@angular/forms";
import {InitiateCategoryViewModel} from "../../../../viewmodels/category/initiate-category.view-model";

@Component({
  selector: 'category-initiate-box',
  templateUrl: 'category-initiate-box.component.html',
  exportAs: 'category-initiate-box'
})

export class CategoryInitiateBoxComponent {

  // Category initiator.
  private initiator: InitiateCategoryViewModel;

  /*
  * Whether component is busy or not.
  * */
  @Input('is-busy')
  private bIsBusy: boolean;

  /*
  * Category initiator box.
  * */
  @ViewChild('categoryInitiateBox')
  private categoryInitiatorBox: NgForm;

  /*
  * Event which should be raised when confirm button is clicked..
  * */
  @Output('click-confirm')
  private eClickConfirm: EventEmitter<InitiateCategoryViewModel>;

  /*
  * Event which should be raised when cancel button is clicked.
  * */
  @Output('click-cancel')
  private eClickCancel: EventEmitter<void>;

  //#region Constructor

  // Initiate component with default dependency injection.
  public constructor() {
    // Initiate initiator.
    this.initiator = new InitiateCategoryViewModel();
    this.eClickConfirm = new EventEmitter<InitiateCategoryViewModel>();
    this.eClickCancel = new EventEmitter<void>();
  }

  //#endregion

  //#region Methods

  /*
  * Reset information.
  * */
  public reset(): void{
    this.initiator = new InitiateCategoryViewModel();
    if (this.categoryInitiatorBox)
      this.categoryInitiatorBox.resetForm();
  }

  //#endregion
}
