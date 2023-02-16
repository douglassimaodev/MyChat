namespace MyChat.Services
{
    public interface IStockService
    {
        Task<Tuple<bool,string>> GetStock(string stockCode);
    }
}
