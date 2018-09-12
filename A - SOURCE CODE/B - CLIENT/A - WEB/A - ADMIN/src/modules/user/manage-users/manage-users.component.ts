import {Component, Inject, OnInit, Input} from '@angular/core';
import {IUserService} from '../../../interfaces/services/user-service.interface';
import {LoadUserViewModel} from '../../../view-models/user/load-user.view-model';
import {Pagination} from '../../../models/pagination';
import {SearchResult} from '../../../models/search-result';
import {User} from '../../../models/entities/user';
import {LazyLoadEvent} from 'primeng/api';
import {TranslateService} from '@ngx-translate/core';
import {BsModalService} from 'ngx-bootstrap';
import {UserDetailComponent} from '../user-detail/user-detail.component';
import {UserDetailModalState} from '../../../models/modal-state/user-detail.modal-state';

@Component({
  selector: 'manage-users',
  templateUrl: 'manage-users.component.html',
  styleUrls: ['manage-users.component.css']
})
export class ManageUsersComponent implements OnInit {

  //#region Properties
  public display: string = 'none';

  public selectedUserId: number;

  public getPersonalProfileId: number;

  public loadUsersCondition: LoadUserViewModel;

  public loadUsersResult: SearchResult<User>;

  //#endregion

  //#region Constructor

  // Initialize component with injectors.
  public constructor(@Inject('IUserService') private userService: IUserService,
                     public bsModalService: BsModalService,
                     private translate: TranslateService) {
    this.loadUsersResult = new SearchResult<User>();

  }

  //#endregion

  //#region Methods

  // Called when component is initialized.
  public ngOnInit(): void {

    this.loadUsersCondition = new LoadUserViewModel();
    let pagination = new Pagination();
    pagination.page = 1;
    pagination.records = 5;
    this.loadUsersCondition.pagination = pagination;

    // Load user using specific conditions.
    this.userService.loadUsers(this.loadUsersCondition)
      .subscribe((loadUsersResult: SearchResult<User>) => {
        this.loadUsersResult = loadUsersResult;
      });
  }

  // Display modal to edit user.
  public editUser(user: User): void {

    // Load user by using id.
    let initialState = new UserDetailModalState();
    initialState.user = user;
    this.bsModalService.show(UserDetailComponent, {initialState: initialState});
  }

  // Display user profile.
  public getPersonalProfile(userId: number): void {
    this.getPersonalProfileId = userId;
  }

  // Load users using specific conditions.
  public loadUsers(event: LazyLoadEvent): void {
    let pagination = new Pagination();

    if (event) {
      pagination.page = event.first / event.rows + 1;
      pagination.records = event.rows;
    } else {
      pagination.page = 1;
      pagination.records = 5;
    }

    this.userService.loadUsers(this.loadUsersCondition)
      .subscribe((loadUsersResult: SearchResult<User>) => {
        this.loadUsersResult = loadUsersResult;
      });
  }

  closeModal() {
    this.display = 'none';
  }

  //#endregion
}
