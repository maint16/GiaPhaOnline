import {Component, EventEmitter, Inject, Input, OnInit, Output} from '@angular/core';
import {IUserService} from '../../../interfaces/services/user-service.interface';
import {ToastrService} from 'ngx-toastr';
import {EditUserStatusViewModel} from '../../../view-models/user/edit-user-status.view-model';
import {TranslateService} from '@ngx-translate/core';
import {User} from '../../../models/entities/user';
import {BsModalRef, BsModalService} from 'ngx-bootstrap';
import {UserStatus} from '../../../enums/user-status.enum';

@Component({
  selector: 'user-detail',
  templateUrl: 'user-detail.component.html',
  styleUrls: ['user-detail.component.css']
})

export class UserDetailComponent implements OnInit {

  //#region Properties

  // User instance to display on modal dialog.
  @Input('user')
  public user: User;

  // List of available statuses.
  public availableUserStatuses = [UserStatus.disabled, UserStatus.active];

  public status = {};
  // _userId: number;
  // get userId(): number {
  //   return this._userId;
  // }
  //
  // @Input('userId')
  // set userId(value: number) {
  //   this._userId = value;
  //   if (this._userId != null) {
  //     this.userService.getUserDetail(this._userId).subscribe((data: any) => {
  //       this.user = new User();
  //       this.user.email = data.email;
  //       this.user.nickname = data.nickname;
  //       this.user.status = data.status;
  //     });
  //   }
  // }

  //#endregion


  //#region Constructor

  // Initialize component with injectors.
  public constructor(@Inject('IUserService') private userService: IUserService,
                     private toastr: ToastrService,
                     private translate: TranslateService,
                     public bsModalRef: BsModalRef) {
    translate.setDefaultLang('en');
  }

  //#endregion

  //#region Methods

  // Called when component is initialized.
  public ngOnInit(): void {
    if (!this.user)
      this.user = new User();
  }

  // Called when OK button is clicked.
  public ngOnOkClicked(): void {
    // User is defined before being passed to component, this means user information should be updated.
    if (!this.user || !this.user.id)
      return;

    let user = this.user;

    let info = new EditUserStatusViewModel();
    info.userId = user.id;
    info.status = user.status;

    this.userService
      .editUserStatus(info)
      .subscribe(() => {
        // Close modal dialog.
        this.bsModalRef.hide();
      });
  }

  // Called when cancel button is clicked on modal.
  public ngOnCancelClicked(): void {
    this.bsModalRef.hide();
  }

  //#endregion
}
