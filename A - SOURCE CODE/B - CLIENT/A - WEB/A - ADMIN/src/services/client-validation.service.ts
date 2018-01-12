import {IDictionary} from "../interfaces/dictionary.interface";
import {IClientValidationService} from "../interfaces/services/client-validation-service.interface";

export class ClientValidationService implements IClientValidationService{

    public validationDictionary : any = {
            'INFORMATION_REQUIRED': 'required'
    };

    /*
    * Build a structure of validation messages sent back from web service.
    * Such as: conditions.pagination.records (Used by .net core web service)
    * */
    private getNetCoreValidation(modelState: any): any{

        // Find list of keys in object.
        let keys = Object.keys(modelState);

        // Initiate properties list after refinement.
        let properties = {};

        // Get through every key in parameter.
        for (let i = 0; i < keys.length; i++){

            // Find the key.
            let key = keys[i];

            // Invalid key.
            if (key == null || key.length < 1)
                continue;

            // Find key in list of keys.
            let camelCaseKey = this.getCamelCasePropertyName(keys[i]);


            // Find validation message of the specific key.
            this.getPropertyValidationMessages(properties, camelCaseKey, modelState[key]);
        }

        return properties;
    }

    /*
     * Build a structure of validation messages sent back from web service.
     * Such as: conditions.pagination.records
     * Scope: one property.
     * */
    private getPropertyValidationMessages(sourceProperty: any, property: string, value: any): any{

        // Split property by character .
        let keys = property.split('.');

        // Initiate pointer.
        let pointer = sourceProperty;

        // Find the last index of keys.
        let lastIndex = keys.length - 1;

        // Get through every key.
        for (let i = 0; i < lastIndex; i++){

            // Find the key.
            let key = keys[i];

            // Key is invalid.
            if (key == null || key.length < 1)
                continue;

            key = this.getCamelCasePropertyName(key);

            // Key already exists in the source property.
            if (pointer[key] != null){
                pointer = pointer[key];
                continue;
            }
            // Extend the property.
            pointer[key] = {};
            pointer = pointer[key];
        }

        // Find the last key of object.
        let lastKey = this.getCamelCasePropertyName(keys[lastIndex]);

        // Key hasn't been specified.
        if (pointer[lastKey] == null){
            pointer[lastKey] = value;
        }
    }

    /*
    * Convert validation property sent back from back-end.
    * */
    /*
     * Input:
     * input = {
     * 	pagination:{
     * 		page: ['INFORMATION_REQUIRED'],
     * 		records: ['RECORD_MIN_INVALID','RECORD_MAX_INVALID']
     * 		}
     * 	};
     *
     * Output:
     * output = {
     *  controls:{
     *      pagination:{
     *          controls:{
     *              page:{
     *                  required: true
     *              },
     *              records:{
     *                  required: true
     *              }
     *          }
     *      }
     *  }
     * }
     * */
    public getFrontendValidationModel(model: any, parameter:any, dictionary: IDictionary<string>): void {

        // Invalid parameter.
        if (parameter == null)
            return;

        if (parameter instanceof Array){

            let errorsList = {};

            for (let index = 0; index < parameter.length; index++){
                let property = parameter[index];

                // Dictionary already contains the translation key.
                if (dictionary.containsKey(property))
                    errorsList[dictionary.getKeyItem(property)] = true;
                else
                    errorsList[property] = true;

                // Mark propery as it has been changed.
                model.markAsDirty();
                model.markAsTouched();

                model.setErrors(errorsList);

            }
            return;
        }

        // Find all properties of parameter.
        let properties = Object.keys(parameter);

        // Iterate through every property of object. (Root is validationModel)
        for (let i = 0; i < properties.length; i++){

            // Find the key which is currently iterated.
            let key = properties[i];

            // Initiate a controls object which contains the iterated property name.
            if (model['controls'] == null) {
                console.log('model doesn\'t have controls');
                continue;
            }


            // Property doesn't exist in the model.
            if (model['controls'][key] == null) {
                console.log(`model["controls"][${key}] doesn't exist`);
                continue;
            }

            console.log(`key = ${key}`);
            //model['controls'][key] = {};
            this.getFrontendValidationModel(model['controls'][key], parameter[key], dictionary);
        }
    };

    /*
    * Find property name in camel-case.
    * */
    public getCamelCasePropertyName(propertyName: string){
            return propertyName.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, (match, index) => {
                if (+match === 0)
                    return '';
                return index == 0 ? match.toLowerCase() : match.toUpperCase();
            });
    }

}
