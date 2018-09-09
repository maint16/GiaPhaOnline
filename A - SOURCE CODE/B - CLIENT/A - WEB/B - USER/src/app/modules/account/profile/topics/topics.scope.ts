import {IScope} from "angular";
import {SearchResult} from "../../../../models/search-result";
import {Topic} from "../../../../models/entities/topic";
import {LoadTopicViewModel} from "../../../../view-models/load-topic.view-model";
import {UrlStateConstant} from "../../../../constants/url-state.constant";
import {Category} from "../../../../models/entities/category";

export interface IPersonalTopicsScope extends IScope {

    //#region Properties

    // Id to category mapping.
    mIdToCategoryMap: {[id: number]: Category};

    urlStateConstant: UrlStateConstant;

    // List of loaded topics.
    loadedPersonalTopics: SearchResult<Topic>;

    // Load topics condition.
    loadPersonalTopicsCondition: LoadTopicViewModel;

    //#endregion

    //#region Methods

    // Called when component is initialized.
    ngOnInit(): void;

    // Called when topics page is changed.
    ngOnTopicsPageChanged(): void;

    //#endregion
}