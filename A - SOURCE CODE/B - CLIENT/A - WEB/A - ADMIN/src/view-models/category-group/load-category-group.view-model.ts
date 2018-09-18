import {Pagination} from '../../models/pagination';

export class LoadCategoryGroupViewModel {

  //#region Properties
  public id: number[];
  public creatorIds: number[];
  public statuses: number[];
  public names: string[];
  public createdTime: {
    from: number,
    to: number
  };
  public pagination: Pagination  = new Pagination();
  //#endregion

}
