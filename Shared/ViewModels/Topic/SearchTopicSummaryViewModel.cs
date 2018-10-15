using System.Collections.Generic;
using Shared.Models;

namespace Shared.ViewModels.Topic
{
    public class SearchTopicSummaryViewModel
    {
        #region Properties

        public HashSet<int> TopicIds { get; set; }
        
        public Pagination Pagination { get; set; }

        #endregion
    }
}