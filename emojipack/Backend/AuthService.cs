using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Flurl;
using Flurl.Http;

namespace emojipack.Backend
{
    public class AuthService
    {
        private ILocalStorageService _storage;
        public AuthService(ILocalStorageService storage)
        {
            _storage = storage;
        }
        public static AuthUser User = null;
        public async Task<bool> RegisterAsync(string username, string password)
        {
            var res = await Program.ApiUrl
                .AppendPathSegments("register")
                .PostJsonAsync(new
            {
                username, password
            });
            return res.ResponseMessage.IsSuccessStatusCode;
        }

        public async Task Resume()
        {
            if (await _storage.ContainKeyAsync("auth"))
            {
                User = await _storage.GetItemAsync<AuthUser>("auth");
                await ApiUtils.RefreshUser();
            }
        }

        public async Task Logout()
        {
            if (await _storage.ContainKeyAsync("auth"))
            {
                await _storage.RemoveItemAsync("auth");
            }

            User = null;
        }

        public async Task<AuthUser> LoginAsync(string username, string password, bool remember)
        {
            var res = await Program.ApiUrl
                .AppendPathSegments("login")
                .PostJsonAsync(new
            {
                username, password
            }).ReceiveJson();
            User = new()
            {
                AccessToken = res.accessToken,
                RefreshToken = res.refreshToken,
                Id = res.id,
                LoginTime = DateTime.UtcNow
            };
            await ApiUtils.RefreshUser();
            if (remember)
            {
                await _storage.SetItemAsync("auth", User);
            }
            return User;
        }
    }
}
