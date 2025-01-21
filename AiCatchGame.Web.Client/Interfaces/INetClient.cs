namespace AiCatchGame.Web.Client.Interfaces
{
    public interface INetClient
    {
        Task<T?> GetAsync<T>(string v);

        Task<TOut?> PostAsync<TIn, TOut>(string url, TIn body);
    }
}