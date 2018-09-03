import {IScope} from "angular";

export interface IRealTimeService {

    //#region Methods

    // Initialize real-time connection.
    initRealTimeConnection();

    // Add listener to a specific channel.
    hookChannel(channelName: string): IScope;

    // Broadcast a message to all local channels.
    addChannelMessage(channelName: string, eventName: string, data: any): void;

    //#endregion
}