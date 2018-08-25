module.exports = (ngModule) => {
    ngModule.service('$topic', ($q) => {

        return {
            loadTopics: () => {
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