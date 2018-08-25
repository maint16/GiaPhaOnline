module.exports = (ngModule) => {
    ngModule.service('$category', ($http) => {

        return {

            /*
            * Load categories with specific conditions.
            * */
            loadCategories: () => {
                return $q((resolve) => {
                    require.ensure([], () => {
                        let items = require('../../assets/mock/load-topics.json');
                        resolve(items);
                    });
                });
            }
        }
    });
};