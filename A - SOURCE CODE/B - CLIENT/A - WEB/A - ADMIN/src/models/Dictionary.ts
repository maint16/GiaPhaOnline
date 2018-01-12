import {IDictionary} from "../interfaces/dictionary.interface";
import {KeyValuePair} from "./key-value-pair";

// Dictionary class.
export class Dictionary<T> implements IDictionary<T> {

  //#region Properties

  // List of dictionary keys.

  // List of key value pair in dictionary
  private keyValuePairs: Array<KeyValuePair<T>>;

  //#endregion

  //#region Constructor

  // Initiate dictionary with default settings.
  public constructor(){
    // Initiate key-value pairs list.
    this.keyValuePairs = new Array<KeyValuePair<T>>();
  }

  //#endregion

  //#region Methods

  /*
  * Insert a new key-value pair into dictionary.
  * */
  public add(key: string, value: T): void {
    // Key exists.
    if (this.containsKey(key)) {
      throw Error(`${key} exists in dictionary`);
    }

    let keyValuePair = new KeyValuePair<T>();
    keyValuePair.key = key;
    keyValuePair.value = value;
    this.keyValuePairs.push(keyValuePair);
  }

  /*
  * Remove a record by using specific key.
  * */
  public remove(key: string) {
    // Find the key in key-value pairs.
    let results = this.keyValuePairs
      .filter((x: KeyValuePair<T>) => {
        return x.key == key;
      })
      .map((x: KeyValuePair<T>) => {
        return this.keyValuePairs.indexOf(x);
      });

    for (let index = 0; index < results.length; index++)
      this.keyValuePairs.splice(index, 1);
  }

  /*
  * Get all keys in dictionary.
  * */
  public keys(): Array<string> {
    return this.keyValuePairs.map((x: KeyValuePair<T>) => {
      return x.key;
    });
  }

  /*
  * Get all values in dictionary.
  * */
  public values(): Array<T> {
    return this.keyValuePairs.map((x: KeyValuePair<T>) => {
      return x.value;
    });
  }

  /*
  * Check whether the designated key has been registered or not.
  * */
  public containsKey(key: string) {
    // Find item using key.
    let item = this.getKeyItem(key);
    if (item == null)
      return false;

    return true;
  }

  /*
  * Find key item.
  * */
  public getKeyItem(key: string): T {
    // Array doesn't contain key value pair before.
    if (this.keyValuePairs == null || this.keyValuePairs.length < 1)
      return null;

    // Filter key-value pairs using key name.
    let results = this.keyValuePairs.filter((x: KeyValuePair<T>) => {
      return x.key == key
    });

    // Key doesn't exist.
    if (results == null || results.length < 1)
      return null;

    return results[0].value;
  }

  /*
  * Get list of key-value pairs in dictionary.
  * */
  public getKeyValuePairs(): Array<KeyValuePair<T>>{
    return this.keyValuePairs;
  }

  //#endregion

}

