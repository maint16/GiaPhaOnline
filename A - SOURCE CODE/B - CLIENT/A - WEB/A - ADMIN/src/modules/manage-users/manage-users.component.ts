import {Component, Inject, OnInit, Input} from '@angular/core';
import {UserViewModel} from '../../view-models/user.view-model';
import {IUserService} from '../../interfaces/services/user-service.interface';
@Component({
  selector: 'manage-users',
  templateUrl: 'manage-users.component.html',
  styleUrls: ['manage-users.component.css']
})
export class ManageUsersComponent implements OnInit {
  public users: [UserViewModel];
  public selectedUserId: number;
  public totalUser: number;
  public constructor(@Inject('IUserService') private userService: IUserService) {
  }
  ngOnInit() {
    this.userService.getUser().subscribe((data: any) => {
      this.users = data;
      var dat = {
        pagination: {
          page: 1,
          records: 5
        }
      };
      this.userService.searchUser(dat).subscribe((data: any) => {
        this.users = data.records;
        this.totalUser = data.total;
      });
    });
  }
    editUser(user) {
      this.selectedUserId = user.id;
    }
    pageChanged($event) {
    }
  }
