module.exports = (ngModule) => {

    /*
    * Initialize service with injectors.
    * */
    ngModule.service('$category', (
        appSettingConstant,
        $http) => {

        return {

            //#region Methods

            /*
            * Load categories by using specific conditions.
            * */
            loadCategories: (condition) => {
                // Build url.
                let fullUrl = `${appSettingConstant.apiEndPoint}/api/category/search`;

                // Load categories list.
                return $http
                    .post(fullUrl, condition)
                    .then((loadCategoriesResponse) => {
                        if (!loadCategoriesResponse)
                            throw 'No category has been found';

                        let loadCategoriesResult = loadCategoriesResponse.data;
                        if (!loadCategoriesResult || !loadCategoriesResult.records)
                            throw 'No category has been found';

                        return loadCategoriesResult;
                    });

            }

            //#endregion
        }

    });
};