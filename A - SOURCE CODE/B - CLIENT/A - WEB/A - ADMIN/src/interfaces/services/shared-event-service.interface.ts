import {EventEmitter} from '@angular/core';

export interface ISharedEventService {

  //#region Methods

  /*
  * Add message channel.
  * */
  addMessageChannel<T>(channelName: string, eventName: string): EventEmitter<T>;

  /*
  * Hook message event.
  * */
  hookMessageChannel<T>(channelName: string, eventName: string): EventEmitter<T>;

  // Publish message to event stream.
  addMessage<T>(channelName: string, eventName: string, data: T): void;

  //#endregion

}
