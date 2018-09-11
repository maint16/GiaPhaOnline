import {Component, Inject, OnInit, Input} from '@angular/core';
import {IUserService} from '../../../interfaces/services/user-service.interface';
import {LoadUserViewModel} from '../../../view-models/user/load-user.view-model';
import {Pagination} from '../../../models/pagination';
import {SearchResult} from '../../../models/search-result';
import {User} from '../../../models/entities/user';
import {LazyLoadEvent} from 'primeng/api';
import {TranslateService} from '@ngx-translate/core';
import * as $ from 'jquery';
@Component({
  selector: 'manage-users',
  templateUrl: 'manage-users.component.html',
  styleUrls: ['manage-users.component.css']
})
export class ManageUsersComponent implements OnInit {

  //#region Properties
  public display : string = 'none';
  public users: User[];
  public selectedUserId: number;
  public getPersonalProfileId: number;
  public totalUser: number;
  public loadUsersCondition: LoadUserViewModel;
  public pagination: Pagination;
  //#endregion

  //#region Constructor

  public constructor(@Inject('IUserService') private userService: IUserService, private translate: TranslateService) {
    translate.setDefaultLang('en');
  }

  //#endregion

  //#region Methods

  // Called when component is initialized.
  public ngOnInit(): void {
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

  // Display modal to edit user.
  public editUser(userId: number): void {
    this.selectedUserId = userId;
    this.display = 'block';
  }

  // Display user profile.
  public getPersonalProfile(userId: number): void {
    this.getPersonalProfileId = userId;
  }

  loadUsersLazy(event: LazyLoadEvent) {    if (this.users) {
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
  closeModal(){
    this.display = 'none';
  }
  //#endregion
}
