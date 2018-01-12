import {Account} from '../../models/entities/Account'
export class CategoryViewModel{

  //#region Properties

  /*
  * Category index.
  * */
  public id: number;

  /*
  * Who created the category.
  * */
  public creator: Account;

  /*
  * Category name.
  * */
  public name: string;

  /*
  * Time when category was created.
  * */
  public createdTime: number;

  /*
  * Time when category was lastly modified.
  * */
  public lastModifiedTime: number;

  //#endregion

}
