import {Post} from "./post";
import {Account} from "./account";

/*
* Post report and its properties.
* */
export class PostReport{

    // Index of post report.
    public id: number;

    // Information of post which is reported.
    public post: Post;

    // Information of post owner.
    public owner: Account;

    // Information of post reporter.
    public reporter: Account;

    // Body of original post.
    public body: string;

    // Reason why the post is reported.
    public reason: string;

    // When the report was created.
    public created: number;
}
