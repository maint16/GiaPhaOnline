import {IDictionary} from "../dictionary.interface";

export interface IClientValidationService{

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
    getFrontendValidationModel(model: any, parameter:any, dictionary: IDictionary<string>): void;
}
