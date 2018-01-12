import {Directive, forwardRef, Attribute, ViewChild} from '@angular/core';
import { Validator, AbstractControl, NG_VALIDATORS } from '@angular/forms';
@Directive({
    selector: '[text-property-comparision][formControlName],[text-property-comparision][formControl],[text-property-comparision][ngModel]',
    providers: [
        {
            provide: NG_VALIDATORS,
            useExisting: forwardRef(() => TextPropertyComparisionValidator),
            multi: true
        }
    ]
})
export class TextPropertyComparisionValidator implements Validator {

    // Initiate validator with default settings.
    public constructor(
        @Attribute('text-property-comparision') public textPropertyComparision: string,
        @Attribute('text-property-comparision-mode') public textPropertyComparisionMode: string) {
    }

    validate(abstractControl: AbstractControl): { [key: string]: any } {

        // Find control value.
        let controlText = abstractControl.value;

        // Find the property whose value is used in comparision.
        let targetProperty = abstractControl.root.get(this.textPropertyComparision);

        // No property is defined.
        if (targetProperty == null)
            return null;

        // Find the value of target control.
        let targetControlText = targetProperty.value;

        targetProperty.valueChanges.subscribe(x => {
            abstractControl.updateValueAndValidity();
        });

        return this.findValidationSummary(controlText, targetControlText);
    }

    // Find validation summary from 2 properties.
    private findValidationSummary(controlText: string, targetControlText: string): any{
        // Base on mode, do comparision.
        switch (this.textPropertyComparisionMode){
            case '<':
                if (controlText < targetControlText)
                    return null;
                break;
            case '<=':
                if (controlText <= targetControlText)
                    return null;
                break;
            case '=':
                if (controlText == targetControlText)
                    return null;
                break;
            case '>=':
                if (controlText >= targetControlText)
                    return null;
                break;
            case '>':
                if (controlText > targetControlText)
                    return null;
                break;
            default:
                if (controlText === targetControlText)
                    return null;
                break;
        }

        // Validation is failed.
        return {
            textPropertyComparision: true
        }
    }
}