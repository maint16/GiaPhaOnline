import {TableModule} from 'primeng/table';
import {Component, Inject, OnInit, Input} from '@angular/core';
import {UserViewModel} from '../../view-models/user.view-model';
import {IUserService} from '../../interfaces/services/user-service.interface';
import {LoadUserViewModel} from '../../view-models/user/load-user.view-model';
import {Pagination} from '../../models/pagination';
import {SearchResult} from '../../models/search-result';
import {User} from '../../models/entities/user';
import {LazyLoadEvent} from 'primeng/api';
@Component({
  selector: 'manage-users',
  templateUrl: 'manage-users.component.html',
  styleUrls: ['manage-users.component.css']
})
export class ManageUsersComponent implements OnInit {
  public users: User[];
  public selectedUserId: number;
  public getPersonalProfileId: number;
  public totalUser: number;
  public loadUsersCondition: LoadUserViewModel;
  public pagination: Pagination;
  public constructor(@Inject('IUserService') private userService: IUserService) {
  }
  ngOnInit() {
    this.loadUsersCondition = new LoadUserViewModel();
    this.pagination = new Pagination();
    this.pagination.page = 1;
    this.pagination.records = 5;
    this.loadUsersCondition.pagination = this.pagination;
    // Load user using specific conditions.
    this.userService.loadUsers(this.loadUsersCondition)
      .subscribe((loadUsersResult: SearchResult<User>) => {
        this.users = loadUsersResult.records;
        this.totalUser = loadUsersResult.total;
      });
  }
    editUser(userId) {
      this.selectedUserId = userId;
    }
  getPersonalProfile(userId) {
    this.getPersonalProfileId = userId;
  }
  loadCarsLazy(event: LazyLoadEvent) {
      if (this.users) {
        this.pagination = new Pagination();
        this.pagination.page = event.first / event.rows + 1;
        this.pagination.records = event.rows;
        this.userService.loadUsers(this.loadUsersCondition)
          .subscribe((loadUsersResult: SearchResult<User>) => {
            this.users = loadUsersResult.records;
            this.totalUser = loadUsersResult.total;
          });
      }
  }
  }
