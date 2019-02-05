namespace OnlineShopV1.Core.Responses
{
    public class GlobalOptions
    {
        
        public int AdminAuthExpireHours { get; set; }

        public GlobalOptions()
        {
            AdminAuthExpireHours = 12;
        }
    }
}