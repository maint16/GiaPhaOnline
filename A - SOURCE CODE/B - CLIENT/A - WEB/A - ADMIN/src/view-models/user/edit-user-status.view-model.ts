import {UserStatus} from '../../enums/user-status.enum';

export class EditUserStatusViewModel {

  //#region Properties

  // Id of user.
  public userId: number;

  // Status to be changed.
  public userStatus: UserStatus;

  //#endregion
}
