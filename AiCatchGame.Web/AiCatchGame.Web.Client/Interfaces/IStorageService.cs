namespace AiCatchGame.Web.Client.Interfaces
{
    public enum LocalStorageKeys
    {
        PlayerPublicId,
        PlayerPrivateId
    }

    public interface IStorageService
    {
        Task<T?> Get<T>(LocalStorageKeys key);

        Task Remove(LocalStorageKeys key);

        Task Set<T>(LocalStorageKeys key, T value);
    }
}