using System.ComponentModel.DataAnnotations;

namespace AppDb.Models.Entities
{
    public class SignalrConnection
    {
        [Required]
        public string ClientId { get; set; }
        
        public int UserId { get; set; }

        public double? LastActivityTime { get; set; }
    }
}