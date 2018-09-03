import {Component, Inject, OnInit} from '@angular/core';
import {UserViewModel} from '../../view-models/user.view-model';
import {IUserService} from '../../interfaces/services/user-service.interface';

@Component({
  selector: 'manage-users',
  templateUrl: 'manage-users.component.html'
})

export class ManageUsersComponent implements OnInit {
  public users: [UserViewModel];
  public constructor(@Inject('IUserService') private userService: IUserService) {
  }
  ngOnInit() {
    this.userService.getUser().subscribe((data: any) => {
      this.users = data;
    });
  }
}
