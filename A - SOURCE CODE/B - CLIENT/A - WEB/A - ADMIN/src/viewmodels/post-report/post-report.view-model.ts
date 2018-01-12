import {Post} from "../../models/entities/post";
import {Account} from "../../models/entities/account";

/*
* Post report view model which is included in find post report.
* */
export class PostReportViewModel{

    // Index of post report.
    public id: number;

    // Post information
    public post: Post;

    // Owner of post report.
    public owner: Account;

    // Reporter information.
    public reporter: Account;

    // Body of report.
    public body: string;

    // Reason of report.
    public reason: string;

    // When the report was created.
    public created: number;
}
