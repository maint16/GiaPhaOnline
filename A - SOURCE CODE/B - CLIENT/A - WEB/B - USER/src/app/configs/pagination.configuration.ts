import {IPaginationConfig} from 'angular-ui-bootstrap'

export class PaginationConfiguration {

    //#region Constructor

    public constructor(uibPaginationConfig: IPaginationConfig){
        uibPaginationConfig.lastText = '>>';
        uibPaginationConfig.nextText = '>';
        uibPaginationConfig.previousText = '<';
        uibPaginationConfig.firstText = '<<';
        uibPaginationConfig.maxSize = 5;
    }

    //#endregion

}