import {Component, Inject, Input, OnInit} from '@angular/core';
import {UserViewModel} from '../../view-models/user.view-model';
import {IUserService} from '../../interfaces/services/user-service.interface';
import {ToastrService} from 'ngx-toastr';
import {SaveUserStatusViewModel} from '../../view-models/user/save-user-status.view-model';
@Component({
  selector: 'user-detail',
  templateUrl: 'user-detail.component.html',
  styleUrls: ['user-detail.component.css']
})

export class UserDetailComponent implements OnInit {
  public user: UserViewModel;
  public statuss = [
    {category: 'Active', value: 1},
    {category: 'Inactive', value: 2}
    ];
  public status = {};
  _userId: number;
  get userId(): number {
    return this._userId;
  }
  @Input('userId')
  set userId(value: number) {
  this._userId = value;
  if (this._userId != null) {
  this.userService.getUserDetail(this._userId).subscribe((data: any) => {
  this.user = new UserViewModel();
  this.user.email = data.email;
  this.user.nickname = data.nickname;
  this.user.status = data.status;
});
}
}
  public constructor(@Inject('IUserService') private userService: IUserService, private toastr: ToastrService) {
  }

  ngOnInit() {
  }
  public saveUserStatus() {
    let saveUserStatusViewModel = new SaveUserStatusViewModel();
    saveUserStatusViewModel.userId = this.user.id;
    saveUserStatusViewModel.userStatus = this.user.status;
  this.userService.saveUserStatus(saveUserStatusViewModel).subscribe((data: any) => {
    this.user = null;
    this.toastr.success('Update Successfully');
  });
  }
  public cancel() {
    this.user = null;
  }
}
