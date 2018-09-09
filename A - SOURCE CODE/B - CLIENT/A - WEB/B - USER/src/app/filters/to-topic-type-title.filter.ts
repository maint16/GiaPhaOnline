import {TopicType} from "../enums/topic-type.enum";

export function toTopicTypeTitle()  {
    return (iTopicType: TopicType): string =>  {
        switch (iTopicType){
            case TopicType.private:
                return 'TITLE_PRIVATE';

            default:
                return 'TITLE_PUBLIC';
        }
    }
}