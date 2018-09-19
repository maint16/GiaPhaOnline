using System;

namespace AppDb.Models.Entities
{
    public class UserRealTimeGroup
    {
        #region Properties

        public Guid Id { get; set; }

        public string Group { get; set; }

        public int UserId { get; set; }
        
        public double CreatedTime { get; set; }

        #endregion
    }
}