module.exports = function(ngModule){
  ngModule.service('postCategorizationService', function($http, appSettings, apiUrls){

      //#region Methods

      /*
      * Get post categorizations by using specific conditions.
      * */
      this.getPostCategorizations = function(conditions){
          var url = appSettings.endPoint.apiService + '/' + apiUrls.postCategorization.getPostCategorization;
          return $http.post(url, conditions);
      };

      //#endregion
  });
};