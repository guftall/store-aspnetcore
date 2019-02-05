namespace OnlineShopV1.Core.Interfaces
{
    public interface IResponse
    {
        int status { get; set; }
        string message { get; set; }
    }
}