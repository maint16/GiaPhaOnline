import {IAngularEvent, IController, IRootElementService} from "angular";
import {User} from "../../../models/entities/user";
import {IProfileScope} from "./profile.scope";
import {IUserService} from "../../../interfaces/services/user-service.interface";
import {IModalService, IModalSettings, IModalInstanceService} from "angular-ui-bootstrap";
import {FileItem, FileUploaderFactory} from 'angular-file-upload';
import {IFileService} from "../../../interfaces/services/file-service.interface";
import {IUiService} from "../../../interfaces/services/ui-service.interface";
import {IToastrService} from "angular-toastr";
import {TokenViewModel} from "../../../view-models/users/token.view-model";
import {ILocalStorageService} from "angular-local-storage";
import {LocalStorageKeyConstant} from "../../../constants/local-storage-key.constant";

export class ProfileController implements IController {

    //#region Properties

    // Upload profile image modal.
    private _uploadProfileImageModal: IModalInstanceService = null;

    // Change password modal.
    private _changePasswordModal: IModalInstanceService = null;

    //#endregion

    //#region Constructor

    public constructor(public profile: User,
                       public $uibModal: IModalService, public toastr: IToastrService,
                       public $translate: angular.translate.ITranslateService, public localStorageService: ILocalStorageService,
                       public $user: IUserService,
                       public $file: IFileService, public $ui: IUiService,
                       public $scope: IProfileScope, public FileUploader: FileUploaderFactory,
                       public $element: IRootElementService){

        // Properties binding.
        $scope.user = profile;
        $scope.availableUserStatuses = $user.loadUserAvailableStatuses();
        $scope.blobProfileImage = null;
        $scope.encodedProfileImage = '';
        $scope.originalProfileImage = '';

        // Profile image uploader.
        let profileImageUploader = new FileUploader({
            removeAfterUpload: true
        });

        profileImageUploader.onAfterAddingFile = this._ngOnAfterAddingProfileImage;
        $scope.profileImageUploader = profileImageUploader;

        // Method binding.
        $scope.ngOnUploadProfileModalClicked = this._ngOnUploadProfileModalClicked;
        $scope.ngOnProfileModalCancelClicked = this._ngOnProfileModalCancelClicked;
        $scope.ngOnProfileImageCropped = this._ngOnProfileImageCropped;
        $scope.ngIsAbleToResetCroppedImage = this._ngIsAbleToResetCroppedImage;
        $scope.ngOnResetOriginalImageClicked = this._ngOnResetOriginalImageClicked;
        $scope.ngOnChangePasswordClicked = this._ngOnChangePasswordClicked;
    }

    //#endregion

    //#region Methods

    // Called when profile upload modal is clicked.
    private _ngOnUploadProfileModalClicked = (): void => {
        let options: IModalSettings = {};
        options.size = 'md';
        options.templateUrl = 'upload-profile-image.html';
        options.scope = this.$scope;

        // Clear image.
        this.$scope.originalProfileImage = '';

        if (this._uploadProfileImageModal)
            this._uploadProfileImageModal.dismiss();

        this._uploadProfileImageModal = this.$uibModal
            .open(options);
    };

    /*
    * Called when a file is added to profile image.
    * */
    private _ngOnAfterAddingProfileImage = (fileItem: any): void => {

        // Add loading screen.
        this.$ui.blockAppUI();

        // File item is invalid.
        let file = fileItem._file;
        this.$file.toEncodedFile(file)
            .then((base64EncodedFile: string| ArrayBuffer) => {
                this.$scope.originalProfileImage = <string> base64EncodedFile;
            })
            .finally(() => {
                this.$ui.unblockAppUI();
            });
    };

    // Called when cancel button of profile image modal is clicked.
    private _ngOnProfileModalCancelClicked = (): void => {

        // Modal is valid. Dimiss it.
        if (!this._uploadProfileImageModal)
            return;

        this._uploadProfileImageModal.dismiss();
        this._uploadProfileImageModal = null;
    };

    // Called when profile image is cropped.
    private _ngOnProfileImageCropped = ($event: IAngularEvent, encodedProfileImage: string, blobProfileImage: any): void => {

        // Prevent form default behavior.
        if ($event)
            $event.preventDefault();

        // Block screen access.
        this.$ui.blockAppUI();

        this.$user.uploadProfileImage(blobProfileImage)
            .then((photo: string) => {
                // Get translated message.
                let message = this.$translate.instant('MSG_PROFILE_IMAGE_UPLOADED_SUCCESSFULLY');
                this.toastr.success(message);

                // Dismiss the modal dialog.
                if (this._uploadProfileImageModal){
                    this._uploadProfileImageModal.dismiss();
                    this._uploadProfileImageModal = null;
                }
            })
            .finally(() => {
                this.$ui.unblockAppUI();
            });
    };

    // Check whether user is able to reset cropped image or not.
    private _ngIsAbleToResetCroppedImage = (originalImage: string): boolean  =>{
        // No image has been selected.
        if (!originalImage)
            return false;

        return true;
    };

    // Called when reset original image is clicked.
    private _ngOnResetOriginalImageClicked = (): void => {
        this.$scope.profileImageUploader.clearQueue();
        let fileUploader = this.$ui.getElement('#profileImageSelector');
        if (fileUploader)
            fileUploader.val(null);

        this.$scope.originalProfileImage = null;
    };

    // Called when change change password is clicked.
    private _ngOnChangePasswordClicked = (): void => {

        if (this._changePasswordModal)
            this._changePasswordModal.dismiss();

        let options: IModalSettings = {};
        options.size = 'md';
        options.templateUrl = 'change-password.html';
        options.controller = 'changePasswordController';

        this._changePasswordModal = this.$uibModal
            .open(options);

        this._changePasswordModal.result
            .then((token: TokenViewModel) => {
                // Update access token.
                this.localStorageService.set<TokenViewModel>(LocalStorageKeyConstant.accessTokenKey, token);

                // Display success message.
                let message = this.$translate.instant('MSG_PAsSWORD_CHANGED_SUCCESSFULLY');
                this.toastr.success(message);
            })
            .catch(() => {
            })
    };

    //#endregion
}