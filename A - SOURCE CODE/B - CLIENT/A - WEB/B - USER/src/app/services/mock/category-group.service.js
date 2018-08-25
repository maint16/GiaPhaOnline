module.exports = (ngModule) => {
    ngModule.service('$categoryGroup', ($q) => {

        return {

            /*
            * Get list of category groups.
            * */
            getCategoryGroups: () => {
                return $q((resolve) => {
                    require.ensure([], () => {
                        let items = require('../../assets/mock/category-groups.json');
                        resolve(items);
                    });
                });
            },

            /*
            * Get category topics.
            * */
            getCategoryTopics: () => {
                return $q((resolve) => {
                    require.ensure([], () => {
                        let items = require('../../assets/mock/topic.json');
                        resolve(items);
                    });
                });
            }
        }
    });
};