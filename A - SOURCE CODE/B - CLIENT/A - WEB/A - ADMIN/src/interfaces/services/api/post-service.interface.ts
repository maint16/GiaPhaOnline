import {SearchPostsViewModel} from "../../../viewmodels/post/search-posts.view-model";
import {Response} from "@angular/http";

export interface IPostService {

    //#region Methods

    // Find posts by using specific conditions.
    getPosts(conditions: SearchPostsViewModel): Promise<Response>;

    // Find post details.
    getPostDetails(index: number): Promise<Response>;

    //#endregion
}
