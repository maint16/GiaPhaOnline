import {Pagination} from '../../models/pagination';

export class LoadCategoryViewModel {

  //#region Properties
  public id: number[];
  public creatorIds: number[];
  public statuses: number[];
  public name: string;
  public createdTime: {
    from: number,
    to: number
  };
  public pagination: Pagination  = new Pagination();
  //#endregion

}
