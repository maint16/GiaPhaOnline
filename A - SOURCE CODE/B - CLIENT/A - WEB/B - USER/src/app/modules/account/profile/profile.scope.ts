import {IAngularEvent, IFormController, IScope} from "angular";
import {User} from "../../../models/entities/user";
import {UserStatus} from "../../../enums/user-status.enum";
import {FileUploader} from "angular-file-upload";
import {UrlStateConstant} from "../../../constants/url-state.constant";

export interface IProfileScope extends IScope {

    //#region Properties

    // Constant import.
    urlStateConstant: UrlStateConstant;

    // User of profile.
    user: User;

    // List of available status.
    availableUserStatuses: Array<UserStatus>;

    // Base 64 of profile image.
    originalProfileImage: string;

    // Profiel image uploader instance.
    profileImageUploader: FileUploader;

    // Result of profile image in base-64.
    encodedProfileImage: string | null;

    // Result of profile image in blob.
    blobProfileImage: any;

    // Profile upload form
    profileImageUploadForm: IFormController;

    //#endregion

    //#region Methods

    // Convert user status to status title.
    ngLoadUserStatusTitle(status: UserStatus): string;

    // Called when upload profile is clicked.
    ngOnUploadProfileModalClicked(): void;

    // Called when profile modal cancel button is clicked.
    ngOnProfileModalCancelClicked(): void;

    // Called when profile image is cropped.
    ngOnProfileImageCropped($event: IAngularEvent, encodedProfileImage: string, blobProfileImage: any): void;

    // Check whether cropped image can be reset or not.
    ngIsAbleToResetCroppedImage(originalImage: string): boolean;

    // Called when reset original image is clicked.
    ngOnResetOriginalImageClicked(): void;

    // Called when change password is clicked.
    ngOnChangePasswordClicked(): void;

    //#endregion

}