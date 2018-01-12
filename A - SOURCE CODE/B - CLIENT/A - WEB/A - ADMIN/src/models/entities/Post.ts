/*
* Post and its properties.
* */
import {Account} from "./account";
import {Category} from "./category";
export class Post{

    // Post index.
    public id: number;

    // Post owner information.
    public owner: Account;

    // Information of category which contains the post.
    public category: Category;

    // Title of post.
    public title: string;

    // Post body
    public body: string;

    // When the post was created.
    public created: number;

    // When the post was lastly modified.
    public lastModified: number;
}
