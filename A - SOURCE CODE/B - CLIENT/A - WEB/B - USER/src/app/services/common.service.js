module.exports = function(ngModule){
  ngModule.service('commonService', function(){

      //#region Methods

      /*
      * From start element index and max records to calculate page.
      * */
      this.getDataTableStartIndex = function(startIndex, maxRecords){
          var iPage = startIndex / maxRecords;
          return Math.ceil(iPage) + 1;
      };

      //#endregion
  });
};