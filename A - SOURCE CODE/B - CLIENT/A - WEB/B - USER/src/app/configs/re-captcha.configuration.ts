import {AppSetting} from "../models/app-setting";

export class ReCaptchaConfiguration {

    //#region Constructor

    public constructor(appSettingConstant: AppSetting,
                       vcRecaptchaServiceProvider: any){

        vcRecaptchaServiceProvider.setSiteKey(appSettingConstant.captchaPublicKey);
    }

    //#endregion

}