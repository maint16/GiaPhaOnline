import {Component, Inject, OnInit, Input} from '@angular/core';
import {UserViewModel} from '../../view-models/user.view-model';
import {IUserService} from '../../interfaces/services/user-service.interface';
import {LoadUserViewModel} from '../../view-models/user/load-user.view-model';
import {Pagination} from '../../models/pagination';
import {SearchResult} from '../../models/search-result';
import {User} from '../../models/entities/user';

@Component({
  selector: 'manage-users',
  templateUrl: 'manage-users.component.html',
  styleUrls: ['manage-users.component.css']
})
export class ManageUsersComponent implements OnInit {
  public p: number = 3;

  public users: User[];
  public selectedUserId: number;
  public totalUser: number;

  public constructor(@Inject('IUserService') private userService: IUserService) {
  }

  ngOnInit() {

    let loadUsersCondition = new LoadUserViewModel();
    let pagination = new Pagination();
    pagination.page = 1;
    loadUsersCondition.pagination = pagination;

    // Load user using specific conditions.
    this.userService.loadUsers(loadUsersCondition)
      .subscribe((loadUsersResult: SearchResult<User>) => {
        this.users = loadUsersResult.records;
        this.totalUser = loadUsersResult.total;
      });
  }

  editUser(user) {
    this.selectedUserId = user.id;
  }

  pageChanged($event) {
  }
}
