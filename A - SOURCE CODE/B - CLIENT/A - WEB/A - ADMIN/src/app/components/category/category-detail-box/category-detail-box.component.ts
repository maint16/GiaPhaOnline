import {Component, EventEmitter, Inject, Input, Output} from '@angular/core';
import * as _ from "lodash";
import {CategoryDetailsViewModel} from "../../../../viewmodels/category/category-details.view-model";
import {ITimeService} from "../../../../interfaces/services/time-service.interface";
import {CategoryViewModel} from "../../../../viewmodels/category/category.view-model";


@Component({
    selector: 'category-detail-box',
    templateUrl: 'category-detail-box.component.html',
    exportAs: 'category-detail-box'
})

export class CategoryDetailBoxComponent{

    //#region Properties

    // Category details which should be displayed on box.
    private details: CategoryViewModel;

    /*
    * Event emitter which should be raised when confirm button is clicked.
    * */
    @Output('click-confirm')
    private eClickConfirm: EventEmitter<CategoryViewModel>;

    /*
    * Event emitter which should be raised when cancel button is clicked.
    * */
    @Output('click-cancel')
    private eClickCancel: EventEmitter<CategoryViewModel>;

    /*
    * Whether component is busy or not.
    * */
    @Input('is-busy')
    private bIsBusy: boolean;

    //#endregion

    //#region Constructor

    // Initiate category detail box with dependency injections.
    public constructor(@Inject('ITimeService') public timeService: ITimeService){
      this.eClickConfirm = new EventEmitter<CategoryViewModel>();
      this.eClickCancel = new EventEmitter<CategoryViewModel>();
    }

    //#endregion

    //#region Methods

    // Set detail information and attach to component.
    public setDetails(details: CategoryViewModel): void{
        this.details = _.cloneDeep(details);
    }

    // Get detail information attached to component.
    public getDetails(): CategoryViewModel{
        return this.details;
    }

    //#endregion
}
