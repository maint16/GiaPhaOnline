import {IScope} from "angular";
import {SearchResult} from "../../../models/search-result";
import {Topic} from "../../../models/entities/topic";
import {LoadTopicViewModel} from "../../../view-models/load-topic.view-model";

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

    //#endregion

    //#region Methods

    /*
    * Called when component is initialized.
    * */
    ngOnInit: () => void;

    /*
    * Called when add topic button is clicked.
    * */
    ngOnAddTopic: () => void;

    /*
    * Called when topic title is clicked.
    * */
    ngOnTopicTitleClicked: (id: number) => void;

    //#endregion

}