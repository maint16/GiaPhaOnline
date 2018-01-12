import {Injectable} from "@angular/core";

/*
* Contains data constraint of application.
* */
@Injectable()
export class ConstraintService{

    /* Category section */
    public minLengthCategoryName: number; // Minimum length of category name.
    public maxLengthCategoryName: number; // Maximum length of category name.

    /* Account section */
    public minLengthEmail: number; // Minimum length of email.
    public maxLengthEmail: number; // Maximum length of email.
    public patternEmail: RegExp; // Regular expression of email.
    public patternAccountPassword: RegExp; // Regular expression of email password.

    /* Token section */
    public patternTokenCode: RegExp; // Regular epxression of token code.

    public constructor(){

        // Category section.
        this.minLengthCategoryName = 6;
        this.maxLengthCategoryName = 64;

        // Account section.
        this.minLengthEmail = 1;
        this.maxLengthEmail = 128;
        this.patternEmail = new RegExp(/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/);
        this.patternAccountPassword = new RegExp(/^[a-zA-Z0-9_!@#$%^&*()]*$/);

        /* Token section */
        this.patternTokenCode = new RegExp(/^[0-9a-f]{32}$/);
    }
}
