import {IScope} from "angular";
import {SearchResult} from "../../../models/search-result";
import {Topic} from "../../../models/entities/topic";
import {LoadTopicViewModel} from "../../../view-models/load-topic.view-model";
import {User} from "../../../models/entities/user";

export interface ITopicsScope extends IScope{

    //#region Properties

    /*
    * List of topics that has been loaded.
    * */
    loadTopicsResult: SearchResult<Topic>;

    /*
    * Condition to load topic.
    * */
    loadTopicsCondition: LoadTopicViewModel;

    /*
    * Mapping between id and user.
    * */
    mIdToUser: {[id: number]: User};

    //#endregion

    //#region Methods

    /*
    * Called when component is initialized.
    * */
    ngOnInit: () => void;

    /*
    * Called when add topic button is clicked.
    * */
    ngOnAddTopicClicked: () => void;

    /*
    * Called when topic title is clicked.
    * */
    ngOnTopicTitleClicked: (id: number) => void;

    /*
    * Called when topics pagination changed.
    * */
    ngOnTopicsPageChanged: () => void;

    // Called when edit topic is clicked.
    ngOnEditTopicClicked: (topicId: number) => void;

    //#endregion

}