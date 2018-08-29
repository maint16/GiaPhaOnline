import {IScope} from "angular";
import {Topic} from "../../../models/entities/topic";
import {SearchResult} from "../../../models/search-result";
import {Reply} from "../../../models/entities/reply";
import {LoadReplyViewModel} from "../../../view-models/load-reply.view-model";

export interface ITopicScope extends IScope {

    //#region Properties

    /*
    * Topic information on page.
    * */
    topic: Topic;

    /*
    * Quick reply content.
    * */
    addQuickReply: string;

    /*
    * Whether editor is in preview mode or not.
    * */
    bIsEditorInPreviewMode: boolean;

    /*
    * Whether editor is collapsed or not.
    * */
    bIsEditorCollapsed: boolean;

    /*
    * Result of loading topic replies.
    * */
    loadTopicRepliesResult: SearchResult<Reply>;

    /*
    * Load topic replies condition.
    * */
    loadTopicRepliesCondition: LoadReplyViewModel;

    //#endregion

    //#region Methods

    /*
    * Called when component is initialized.
    * */
    ngOnInit: () => void;

    /*
    * Called when add reply is clicked.
    * */
    ngOnAddReplyClicked: (content: string) => void;

    /*
    * Called when editor toggle button is clicked.
    * */
    ngOnToggleEditorPreviewMode: () => void;

    /*
    * Called when replies page is changed.
    * */
    ngOnRepliesPageChanged: () => void;

    /*
    * Called when on editor toggle is clicked.
    * */
    ngOnEditorToggleClicked: () => void;

    /*
    * Called when add topic button is clicked.
    * */
    ngOnAddTopicReplyClicked: () => void;

    //#endregion
}