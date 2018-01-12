import {TextSearchMode} from "../enumerations/text-search-mode";

/*
* Structure of text searching parameter.
* */
export class TextSearch{

    // String which is used for searching.
    public value: string;

    // Text searching mode.
    public mode: TextSearchMode;

    // Initiate text searching parameter.
    public constructor(){
        this.value = null;
        this.mode = TextSearchMode.contains;
    }
}
