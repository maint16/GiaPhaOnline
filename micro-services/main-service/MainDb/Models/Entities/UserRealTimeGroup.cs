using System;

namespace MainDb.Models.Entities
{
    public class UserRealTimeGroup
    {
        #region Properties

        public Guid Id { get; set; }

        public string Group { get; set; }

        public int UserId { get; set; }

        public double CreatedTime { get; set; }

        #endregion

        #region Constructor

        public UserRealTimeGroup()
        {
        }

        public UserRealTimeGroup(Guid id, string group, int userId, double createdTime)
        {
            Id = id;
            Group = group;
            UserId = userId;
            CreatedTime = createdTime;
        }

        #endregion
    }
}