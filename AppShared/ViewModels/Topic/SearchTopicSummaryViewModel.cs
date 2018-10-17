using System.Collections.Generic;
using AppShared.Models;

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