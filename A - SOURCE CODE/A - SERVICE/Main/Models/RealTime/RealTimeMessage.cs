namespace Main.Models.RealTime
{
    public class RealTimeMessage<T>
    {
        #region Properties

        public string Header { get; set; }

        public string Body { get; set; }
        
        public T AdditionalInfo { get; set; }

        #endregion
    }
}