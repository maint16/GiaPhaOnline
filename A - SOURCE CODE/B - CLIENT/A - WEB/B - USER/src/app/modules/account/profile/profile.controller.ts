import {IController} from "angular";
import {User} from "../../../models/entities/user";
import {IProfileScope} from "./profile.scope";
import {IUserService} from "../../../interfaces/services/user-service.interface";

export class ProfileController implements IController {

    //#region Constructor

    public constructor(public profile: User,
                       public $user: IUserService,
                       public $scope: IProfileScope){
        $scope.user = profile;
        $scope.availableUserStatuses = $user.loadUserAvailableStatuses();
        $scope.ngLoadUserStatusTitle = $user.loadUserStatusTitle;
    }

    //#endregion

    //#region Methods

    //#endregion
}