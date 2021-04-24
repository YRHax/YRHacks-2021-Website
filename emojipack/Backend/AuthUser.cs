using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace emojipack.Backend
{
    public class AuthUser
    {
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime LoginTime { get; set; }
        
        public List<Pack> Packs { get; set; } = new List<Pack>();

        public async Task DeletePack(Pack pack)
        {
            if (Program.TESTING)
            {
                Packs.Remove(pack);
                EventService.InvokeHoverEmojiChangedEvent();
                return;
            }

            Packs.Remove(pack);
            await Program.ApiUrl
                .AppendPathSegments("pack", "delete",pack.PackId)
                .WithOAuthBearerToken(AccessToken)
                .DeleteAsync()
                .ReceiveJson();
        }

        public async Task<Pack> CreatePack(string name)
        {
            if (Program.TESTING)
            {
                var cpack = new Pack()
                {
                    PackId = Guid.NewGuid().ToString(),
                    PackName = name,
                    Emojis = new List<Emoji>(),
                    PackCount = 0,
                    PackOwner = Id
                };
                //await ApiUtils.RefreshPack(cpack);
                Packs.Add(cpack);
                return cpack;
            }


            Debug.Assert(!string.IsNullOrEmpty(name));
            var res = await Program.ApiUrl
                .AppendPathSegments("pack", "create")
                .WithOAuthBearerToken(AccessToken)
                .PostJsonAsync(new
                {
                    name,
                    Id
                })
                .ReceiveJson();
            var pack = new Pack()
            {
                PackId = res.id,
                PackOwner = Id
            };
            await ApiUtils.RefreshPack(pack);
            Packs.Add(pack);
            return pack;
        }

        public async Task<Pack> ClonePack(string id, string name)
        {
            var res = await Program.ApiUrl
                .AppendPathSegments("pack", "clone")
                .WithOAuthBearerToken(AccessToken)
                .PostJsonAsync(new
                {
                    srcId = id,
                    userId = Id
                });
            if (res.ResponseMessage.IsSuccessStatusCode)
            {
                var pack = new Pack()
                {
                    PackId = (await res.GetJsonAsync()).id,
                    PackOwner = Id
                };
                await ApiUtils.RefreshPack(pack);
                await pack.RenamePack(name);
                Packs.Add(pack);
                return pack;
            }
            else
            {
                return null;
            }
        }
    }
}
