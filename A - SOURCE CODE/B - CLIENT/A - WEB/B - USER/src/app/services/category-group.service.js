module.exports = (ngModule) => {
    ngModule.service('$categoryGroup',
        (appSettingConstant,
         $http) => {
            return {

                //#region Methods

                /*
                * Load category groups using specific conditions.
                * */
                loadCategoryGroups: (condition) => {
                    // Construct url.
                    let url = `${appSettingConstant.apiEndPoint}/api/category-group/search`;
                    return $http
                        .post(url, condition)
                        .then((loadCategoryGroupsResponse) => {
                            if (!loadCategoryGroupsResponse)
                                throw 'No category group has been found';

                            let loadCategoryGroupsResult = loadCategoryGroupsResponse.data;
                            if (!loadCategoryGroupsResult || !loadCategoryGroupsResult.records)
                                throw 'No category group has been found';

                            return loadCategoryGroupsResult;
                        });
                }

                //#endregion

            }
        });
};