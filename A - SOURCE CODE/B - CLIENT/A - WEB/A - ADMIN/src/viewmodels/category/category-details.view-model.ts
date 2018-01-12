import {Account} from "../../models/entities/account";

/*
* Category and its detailed information.
* */
export class CategoryDetailsViewModel{
    // Id of category.
    public id: number;

    // Information of category creator.
    public creator: Account;

    // Category name
    public name: string;

    // When the category was created.
    public created: number;

    // When the category was lastly modified.
    public lastModified: number;
}
