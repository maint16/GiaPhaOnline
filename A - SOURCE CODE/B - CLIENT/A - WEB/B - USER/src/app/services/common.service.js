module.exports = function (ngModule) {
    ngModule.service('commonService', function (blockUI) {

        //#region Methods

        /*
        * From start element index and max records to calculate page.
        * */
        this.getDataTableStartIndex = function (startIndex, maxRecords) {
            var iPage = startIndex / maxRecords;
            return Math.ceil(iPage) + 1;
        };

        /*
        * Split array into chunks.
        * */
        this.splitArrayIntoChunks = function (source, chunkSize) {
            var arrayLength = source.length;
            var chunks = [];

            for (var index = 0; index < arrayLength; index += chunkSize) {
                var chunk = myArray.slice(index, index + chunkSize);

                chunks.push(chunk);
            }

            return chunks;
        };

        /*
        * Block application UI.
        * */
        this.blockAppUI = function(){
            var appBlockUI = blockUI.instances.get('appBlockUI');
            if (!appBlockUI)
                return;

            appBlockUI.start();
        };

        /*
        * Unblock application UI.
        * */
        this.unblockAppUI = function(){
            var appBlockUI = blockUI.instances.get('appBlockUI');
            if (!appBlockUI)
                return;

            appBlockUI.stop();
        };

        //#endregion
    });
};