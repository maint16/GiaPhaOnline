// Dictionary interface which is used implementing key-value data type.
import {KeyValuePair} from "../models/key-value-pair";

export interface IDictionary<T> {

    //#region Methods

    // Add a key-value record to dictionary.
    add(key: string, value: T): void;

    // Remove record by using key.
    remove(key: string): void;

    // Whether dictionary contains key or not.
    containsKey(key: string): boolean;

    // Get all keys in dictionary.
    keys(): string[];

    // Get all value in dictionary.
    values(): T[];

    // Get list of key value pairs in dictionary.
    getKeyValuePairs(): Array<KeyValuePair<T>>;

    // Find key item.
    getKeyItem(key: string): T;

    //#endregion
}
