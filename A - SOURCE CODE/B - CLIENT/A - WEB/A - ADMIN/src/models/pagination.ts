import {ApplicationSetting} from "../constants/application-setting";

export class Pagination {

  //#region Properties

  /*
  * Page number.
  * */
  public page: number;

  /*
  * Amount of records which should be displayed on the screen.
  * */
  public records: number;

  //#endregion

  //#region constructor

  /*
  * Initiate class with default settings.
  * */
  public constructor(){
    this.page = 1;
    this.records = ApplicationSetting.maxPageRecords;
  }

  //#endregion
}
