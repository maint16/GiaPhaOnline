import {SearchCommentsViewModel} from "../../../viewmodels/comment/search-comments.view-model";
import {Response} from "@angular/http";
import {SearchCommentsDetailsViewModel} from "../../../viewmodels/comment/search-comments-details.view-model";

export interface ICommentService {

    //#region Methods

    // Get list of comments base on specific conditions.
    getComments(conditions: SearchCommentsViewModel): Promise<Response>;

    // Search for a specific comment's detail.
    getCommentDetails(conditions: SearchCommentsDetailsViewModel): Promise<Response>;

    //#endregion
}
