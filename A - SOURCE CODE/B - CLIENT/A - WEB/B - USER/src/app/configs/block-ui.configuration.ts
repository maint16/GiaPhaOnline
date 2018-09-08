export class BlockUiConfiguration {

    //#region Properties

    public constructor(blockUiConfig: angular.blockUI.BlockUIConfig){
        // BlockUI configuration.
        blockUiConfig.autoInjectBodyBlock = false;
        blockUiConfig.templateUrl = 'main-block-ui.html';
    }

    //#endregion
}