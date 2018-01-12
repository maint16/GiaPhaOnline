import {ToastPosition} from "../enumerations/toastr-position";


/*
 * Configuration of toast.
 * */
export class ToastOptions {

    //#region Properties

    // Whether close button should appear in toast.
    public closeButton: boolean;

    // Whether debug mode is on.
    public debug: boolean;

    // Whether newest toast be on top.
    public newestOnTop: boolean;

    // Whether progress bar should be displayed or not.
    public progressBar: boolean;

    // Whether similar toast cannot be displayed or not.
    public preventDuplicates: boolean;

    // Life-time of toast.
    public showDuration: number;

    // How many milliseconds does it take to make toast disappear.
    public hideDuration: number;

    // Timeout of toast.
    public timeOut: number;

    public extendedTimeOut: number;

    // Mode of easing.
    public showEasing: string;

    // Easing hide mode.
    public hideEasing: string;

    // Display method.
    public showMethod: string;

    // Way that toast disappear.
    public hideMethod: string;

    // Whether toast can be dismissed by tapping.
    public tapToDismiss: boolean;

    // Place where toast should be shown.
    public position: ToastPosition;

    //#endregion

    //#region Constructor

    // Intiate toast option with default settings.
    public constructor() {
        this.closeButton = true;
        this.debug = false;
        this.newestOnTop = true;
        this.progressBar = false;
        this.preventDuplicates = true;
        this.showDuration = 300;
        this.hideDuration = 1000;
        this.timeOut = 0;
        this.extendedTimeOut = 0;
        this.showEasing = 'swing';
        this.hideEasing = 'linear';
        this.showMethod = 'fadeIn';
        this.hideMethod = 'fadeOut';
        this.tapToDismiss = true;
        this.position = ToastPosition.topRight;
    }

    //#endregion
}
