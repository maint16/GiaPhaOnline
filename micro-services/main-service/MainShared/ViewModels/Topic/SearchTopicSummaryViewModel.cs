using System.Collections.Generic;
using ClientShared.Models;

namespace MainShared.ViewModels.Topic
{
    public class SearchTopicSummaryViewModel
    {
        #region Properties

        public HashSet<int> TopicIds { get; set; }

        public Pagination Pagination { get; set; }

        #endregion
    }
}