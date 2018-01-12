import {ITimeService} from "../interfaces/services/time-service.interface";

/*
* Client time service which handles time calculation.
* */
export class TimeService implements ITimeService{

  //#region Methods

    // Convert number to unix time.
    getUtc(milliseconds: number): Date {

        // Initiate date and set its millisecs.
        let date = new Date();
        date.setTime(milliseconds);
        return date;
    }

    //#endregion
}
