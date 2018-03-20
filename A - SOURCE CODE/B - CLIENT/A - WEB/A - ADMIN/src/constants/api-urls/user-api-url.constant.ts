export class UserApiUrlConstant{

  //#region Properties

  /*
  * Url which is for exchanging user information for access token.
  * */
  public static basicLogin: string = 'user/basic-login';

  /*
  * Url which is for exchanging search conditions for users list.
  * */
  public static getUsers: string = 'user/search-users'

  /*
  * Url which is for getting user personal profile.
  * */
  public static getPersonalProfile: string = 'user/personal-profile';

  //#endregion
}