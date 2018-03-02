module.exports = function(ngModule){

    ngModule.service('uiService', function(){

        //#region Methods

        /*
        * Trigger windows resize function.
        * */
        this.reloadWindowSize = function(){
            $(window).resize();
            console.log('Resize');
        };

        /*
        * Find element in dom.
        * */
        this.getElement = function(elementQuery){
            return $(elementQuery);
        };

        //#endregion
    });
};