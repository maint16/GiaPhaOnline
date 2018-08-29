module.exports = (ngModule) => {

    ngModule.service('$ui', (blockUI) => {

        return {

            //#region Methods

            // Block application UI.
            blockAppUI: () => {
                const appBlockUI = blockUI.instances.get('appBlockUI');
                if (!appBlockUI)
                    return;

                appBlockUI.start();
            },

            // Unblock application UI.
            unblockAppUI: () => {
                const appBlockUI = blockUI.instances.get('appBlockUI');
                if (!appBlockUI)
                    return;

                appBlockUI.stop();
            }

            //#endregion
        }
    });
};