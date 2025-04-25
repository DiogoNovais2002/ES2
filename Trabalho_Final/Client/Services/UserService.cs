using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Client.Services
{
    public class UserService
    {
        private readonly IJSRuntime _js;

        public UserService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<(int? UserId, string? UserType)> GetUserDataAsync()
        {
            try
            {
                var type = await _js.InvokeAsync<string>("localStorage.getItem", "userType");
                var idStr = await _js.InvokeAsync<string>("localStorage.getItem", "userId");
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(idStr))
                    return (null, null);

                if (int.TryParse(idStr, out var id))
                    return (id, type);

                return (null, null);
            }
            catch
            {
                return (null, null);
            }
        }
    }
}