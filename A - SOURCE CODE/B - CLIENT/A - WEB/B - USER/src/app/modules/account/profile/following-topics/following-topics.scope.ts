import {IScope} from "angular";
import {Topic} from "../../../../models/entities/topic";
import {LoadFollowingTopicViewModel} from "../../../../view-models/following-topic/load-following-topic.view-model";
import {SearchResult} from "../../../../models/search-result";
import {FollowingTopic} from "../../../../models/entities/following-topic";

export interface IFollowingTopicsScope extends IScope {

    //#region Properties

    // Condition to load following topic.
    loadFollowingTopicCondition: LoadFollowingTopicViewModel;

    // Loaded following topic.
    loadedFollowingTopic: SearchResult<FollowingTopic>;

    //#endregion

    //#region Methods

    // Called when component is initialized.
    ngOnInit(): void;

    // Called when page is changed.
    ngOnPageChanged(): void;

    // Called when topic is clicked.
    ngOnTopicClicked(topic: Topic): void;

    // Get topic information.
    ngGetTopic(id: number): Topic;

    //#endregion
}