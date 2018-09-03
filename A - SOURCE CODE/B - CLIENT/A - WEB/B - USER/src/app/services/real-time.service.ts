import {IRealTimeService} from "../interfaces/services/real-time-service.interface";
import {IPromise, IQService, IScope} from "angular";
import Pusher = require('pusher-js');
import {AppSetting} from "../models/app-setting";
import * as pusher from "pusher-js";
import {Channel} from "pusher-js";

/* @ngInject */
export class RealTimeService implements IRealTimeService {

    //#region Properties

    // Event subscriber.
    private _subscriber: pusher.Pusher = null;

    // Channel - scope mapping.
    private _mChannelScopeMap: { [name: string]: Channel } = {};

    //#endregion

    //#region Constructor

    // Initialize service with injectors.
    public constructor(public appSettingConstant: AppSetting,
                       public $q: IQService) {
    }

    //#endregion

    //#region Methods

    // Hook to a channel to catch its message.
    public hookChannel(channelName: string): pusher.Channel {
        // Subscriber hasn't been initialized.
        if (!this._subscriber)
            return null;

        if (!this._mChannelScopeMap[channelName])
            this._mChannelScopeMap[channelName] = this._subscriber.subscribe(channelName);

        return this._mChannelScopeMap[channelName];
    }

    // Initialize real-time connection.
    public initRealTimeConnection(): IPromise<void> {

        return this.$q<void>((resolve, reject) => {
            // Subscriber has been registered.
            if (this._subscriber)
                return;

            this._subscriber = new Pusher(this.appSettingConstant.apiKeyPusher, {
                cluster: this.appSettingConstant.clusterPusher
            });

            // Called when channel is connected.
            this._subscriber.connection.bind('connected',  resolve);
            this._subscriber.connection.bind('unavailable',  reject);
            this._subscriber.connection.bind('failed', reject);
            this._subscriber.connection.bind('disconnected', reject);
        });
    }

    //#endregion

}