using System.Collections.Generic;
using ClientShared.Models;

namespace AppShared.ViewModels.Topic
{
    public class SearchTopicSummaryViewModel
    {
        #region Properties

        public HashSet<int> TopicIds { get; set; }

        public Pagination Pagination { get; set; }

        #endregion
    }
}