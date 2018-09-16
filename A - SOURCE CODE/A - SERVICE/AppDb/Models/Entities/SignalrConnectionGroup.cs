using System;
using System.ComponentModel.DataAnnotations;

namespace AppDb.Models.Entities
{
    public class SignalrConnectionGroup
    {
        #region Properties
        
        public Guid Id { get; set; }

        public string ClientId { get; set; }

        public string Group { get; set; }

        public int UserId { get; set; }
        
        #endregion
    }
}