namespace Main.Models.RealTime
{
    public class RealTimeMessage<T>
    {
        #region Properties

        public string Title { get; set; }

        public string Body { get; set; }
        
        public T AdditionalInfo { get; set; }

        #endregion

        #region Constructors

        public RealTimeMessage()
        {
            
        }

        public RealTimeMessage(string title, string body, T additionalInfo)
        {
            Title = title;
            Body = body;
            AdditionalInfo = additionalInfo;
        }

        public RealTimeMessage(T additionalInfo)
        {
            AdditionalInfo = additionalInfo;
        }


        #endregion
    }
}