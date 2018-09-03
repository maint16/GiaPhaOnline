import {IController, IHttpResponse, IPromise} from "angular";
import {ITopicService} from "../../../interfaces/services/topic-service.interface";
import {ITopicScope} from "./topic.scope";
import {TopicResolver} from "./topic.resolver";
import {StateService} from "@uirouter/core";
import {Reply} from "../../../models/entities/reply";
import {SearchResult} from "../../../models/search-result";
import {LoadReplyViewModel} from "../../../view-models/load-reply.view-model";
import {Pagination} from "../../../models/pagination";
import {PaginationConstant} from "../../../constants/pagination.constant";
import {IUiService} from "../../../interfaces/services/ui-service.interface";
import {IReplyService} from "../../../interfaces/services/reply-service.interface";

import {cloneDeep} from 'lodash';
import {Topic} from "../../../models/entities/topic";
import {AddReplyViewModel} from "../../../view-models/add-reply.view-model";
import {User} from "../../../models/entities/user";
import {LoadUserViewModel} from "../../../view-models/users/load-user.view-model";
import {IUserService} from "../../../interfaces/services/user-service.interface";
import {UserRole} from "../../../enums/user-role.enum";
import {UrlStateConstant} from "../../../constants/url-state.constant";

/* @ngInject */
export class TopicController implements IController {

    //#region Properties


    //#endregion

    //#region Constructor

    public constructor(public $topic: ITopicService, public $reply: IReplyService, public $user: IUserService,
                       public $ui: IUiService,
                       loadTopicResolver: TopicResolver,
                       public $state: StateService,
                       public $scope: ITopicScope) {

        // Property binding.
        let topic = loadTopicResolver.topic;
        $scope.topic = topic;
        $scope.addQuickReply = '';
        $scope.bIsEditorInPreviewMode = false;
        $scope.bIsEditorCollapsed = false;
        $scope.loadTopicRepliesResult = new SearchResult<Reply>();

        // Initialize search condition.
        let loadTopicRepliesCondition = new LoadReplyViewModel();
        loadTopicRepliesCondition.topicIds = [topic.id];
        loadTopicRepliesCondition.pagination = new Pagination();
        loadTopicRepliesCondition.pagination.page = 1;
        loadTopicRepliesCondition.pagination.records = PaginationConstant.topicReplies;
        $scope.loadTopicRepliesCondition = loadTopicRepliesCondition;

        // Method binding.
        $scope.ngOnAddReplyClicked = this._ngOnAddReplyClicked;
        $scope.ngOnInit = this._ngOnInit;
        $scope.ngOnToggleEditorPreviewMode = this._ngOnToggleEditorPreviewMode;
        $scope.ngOnRepliesPageChanged = this._ngOnRepliesPageChanged;
        $scope.ngOnEditorToggleClicked = this._ngOnEditorToggleClicked;
        $scope.ngOnAddReplyClicked = this._ngOnAddTopicReplyClicked;
        $scope.ngOnProfileClicked = this._ngOnProfileClicked;
    }

    //#endregion

    //#region Methods

    /*
    * Called when component is initialized.
    * */
    private _ngOnInit = (): void => {
        this._ngOnRepliesPageChanged();
    };

    /*
    * Called when add reply is clicked.
    * */
    private _ngOnAddReplyClicked = (): void => {

    };

    /*
    * Called when preview toggle button is clicked.
    * */
    private _ngOnToggleEditorPreviewMode = (): void => {
        this.$scope.bIsEditorInPreviewMode = !this.$scope.bIsEditorInPreviewMode;
    };

    /*
    * Called when replies page is changed.
    * */
    private _ngOnRepliesPageChanged = (): void => {

        // Add loading screen.
        this.$ui.blockAppUI();

        // Caching result
        let loadReplyResult = new SearchResult<Reply>();
        let mIdToUserMap: { [id: number]: User } = {};

        this._loadTopicReplies()
            .then((loadTopicRepliesResult: SearchResult<Reply>) => {
                loadReplyResult = loadTopicRepliesResult;
                return loadTopicRepliesResult.records;
            })
            .catch(() => {
                return new Array<Reply>();
            })
            .then((replies: Reply[]) => {
                let loadUserConditions = new LoadUserViewModel();
                let ownerIds = replies.map((reply: Reply) => reply.ownerId);
                loadUserConditions.ids = [this.$scope.topic.ownerId].concat(ownerIds);

                this.$user
                    .loadUsers(loadUserConditions)
                    .then((loadUsersResult: SearchResult<User>) => {
                        // Get users list.
                        let users = loadUsersResult.records;
                        for (let user of users)
                            mIdToUserMap[user.id] = user;
                    });
            })
            .finally(() => {
                this.$scope.mIdToUserMap = mIdToUserMap;
                this.$scope.loadTopicRepliesResult = loadReplyResult;
                this.$ui.unblockAppUI();
            })
    };

    /*
    * Called when editor toggle is clicked.
    * */
    private _ngOnEditorToggleClicked = (): void => {
        this.$scope.bIsEditorCollapsed = !this.$scope.bIsEditorCollapsed;
    };

    /*
    * Called when add topic reply is clicked.
    * */
    private _ngOnAddTopicReplyClicked = (content: string): void => {

        // Block ui.
        this.$ui.blockAppUI();

        // Initialize model.
        let model = new AddReplyViewModel();
        model.topicId = this.$scope.topic.id;
        model.content = this.$scope.addQuickReply;

        this.$reply
            .addReply(model)
            .then((reply: Reply) => {
                // Clear model.
                this.$scope.addQuickReply = '';
            })
            .finally(() => {
                this.$ui.unblockAppUI();
            });
    };

    // Called when profile is clicked.
    private _ngOnProfileClicked = (profileId: number): void => {
        this.$state.go(UrlStateConstant.profileModuleName, {profileId: profileId});
    };

    /*
    * Load topic replies base on the existing condition.
    * */
    private _loadTopicReplies = (): IPromise<SearchResult<Reply>> => {
        // Load topic replies.
        let loadTopicRepliesCondition = cloneDeep(this.$scope.loadTopicRepliesCondition);
        return this.$reply
            .loadReplies(loadTopicRepliesCondition);
    }

    //#endregion
}