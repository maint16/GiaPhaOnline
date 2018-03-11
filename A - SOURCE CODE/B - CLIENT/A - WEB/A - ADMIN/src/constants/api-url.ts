export class ApiUrl {

  //#region Accounts

  /*
  * Url which is for logging into system.
  * */
  public static login: string = 'api/account/internal-login';

  /*
  * Url which is for search accounts by using specific conditions.
  * */
  public static getAccounts: string = 'api/account/search';

  /*
  * Url which is for editing account information.
  * */
  public static editAccount: string = 'api/account';

  /*
  * Url which is for submitting account password.
  * */
  public static initPasswordSubmission: string = 'api/account/submit-password';

  /*
  * Url which is for requesting back-end service to reset password.
  * */
  public static requestPasswordReset: string = 'api/account/reset-password';

  /*
  * Url which is for getting account profile.
  * */
  public static getAccountProfile: string = 'api/user/personal-profile/{id}';

  //#endregion

  //#region Categories

  /*
  * Url which is for searching for categories.
  * */
  public static getCategories: string = "api/category/search";

  /*
  * Url which is for deleting for categories.
  * */
  public static deleteCategory: string = "api/category";

  /*
  * Url which is for changing for category detail.
  * */
  public static editCategoryDetails: string = "api/category";

  /*
  * Url which is for initiating category.
  * */
  public static initiateCategory: string = "api/category";


  //#endregion

//#region Comments

  /*
  * Url which is for searching comments.
  * */
  public static getComments: string = "api/comment/find";

  /*
  * Url which is for searching for comment details.
  * */
  public static getCommentsDetails: string = "api/comments/details";

  //#endregion

  //#region Posts

  /*
  * Search posts.
  * */
  public static getPosts: string = "api/post/search";

  /*
  * Search post details.
  * */
  public static getPostsDetails: string = "api/post/details";

  //#endregion

  //#region Post report

  /*
  * Url which is for searching for post reports.
  * */
  public static getPostReports: string = "api/report/post/find";

  /*
  * Url which is for deleting for post reports.
  * */
  public static deletePostReport: string = "api/report/post";


  //#endregion
}
