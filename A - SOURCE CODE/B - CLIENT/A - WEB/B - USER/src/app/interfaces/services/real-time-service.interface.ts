import {IPromise, IScope} from "angular";
import {Channel} from "pusher-js";

export interface IRealTimeService {

    //#region Methods

    // Initialize real-time connection.
    initRealTimeConnection(): IPromise<void>;

    // Add listener to a specific channel.
    hookChannel(channelName: string): Channel;

    //#endregion
}