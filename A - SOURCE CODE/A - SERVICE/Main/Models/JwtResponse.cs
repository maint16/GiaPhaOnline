namespace Main.Models
{
    public class JwtResponse
    {
        /// <summary>
        /// Access code which is used for accessing into system.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Life-time of 
        /// </summary>
        public int LifeTime { get; set; }

        /// <summary>
        /// When token should be expired.
        /// </summary>
        public double Expiration { get; set; }
    }
}