using AiCatchGame.Web.Client.Interfaces;
using Blazored.LocalStorage;

namespace AiCatchGame.Web.Client.Services
{
    public class StorageService : IStorageService
    {
        private ILocalStorageService _localStorage;

        public StorageService(ILocalStorageService localStorageService)
        {
            _localStorage = localStorageService;
        }

        public async Task<T?> Get<T>(LocalStorageKeys key)
        {
            return await _localStorage.GetItemAsync<T>(key.ToString());
        }

        public async Task Set<T>(LocalStorageKeys key, T value)
        {
            await _localStorage.SetItemAsync(key.ToString(), value);
        }
    }
}