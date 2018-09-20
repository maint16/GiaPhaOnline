namespace Main.Models.RealTime
{
    public class RealTimeMessage<T>
    {
        #region Properties

        public string Title { get; set; }

        public string Body { get; set; }
        
        public string Icon { get; set; }

        public T AdditionalInfo { get; set; }

        #endregion

        #region Constructors

        public RealTimeMessage()
        {
        }

        public RealTimeMessage(string title, string body, string icon, T additionalInfo)
        {
            Title = title;
            Body = body;
            Icon = icon;
            AdditionalInfo = additionalInfo;
        }

        public RealTimeMessage(T additionalInfo)
        {
            AdditionalInfo = additionalInfo;
        }

        #endregion
    }
}