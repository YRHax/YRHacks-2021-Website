using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace emojipack.Backend
{
    public class QueryService
    {
        public async Task<List<Emoji>> QueryTop100()
        {
            var res = await Program.ApiUrl.AppendPathSegments("query", "top")
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .PostAsync()
                .ReceiveJsonList();
            List<Emoji> emojis = new List<Emoji>();
            foreach (var k in res)
            {
                var e = new Emoji()
                {
                    ClickCount = k.count,
                    EmojiName = k.name,
                    EmojiId = k.id,
                    EmojiPackId = k.packId,
                    EmojiOwnerId = ""
                };
                emojis.Add(e);
            }

            return emojis;
        }

        public async Task<List<Emoji>> QueryGlobal(string query)
        {
            var res = await Program.ApiUrl.AppendPathSegments("query", "global")
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .PostJsonAsync(new
                {
                    query
                })
                .ReceiveJsonList();
            List<Emoji> emojis = new List<Emoji>();
            foreach (var k in res)
            {
                var e = new Emoji()
                {
                    ClickCount = k.count,
                    EmojiName = k.name,
                    EmojiId = k.id,
                    EmojiPackId = k.packId,
                    EmojiOwnerId = ""
                };
                emojis.Add(e);
            }

            return emojis;
        }

        public async Task<List<Pack>> QueryPacks(string query)
        {
            var res = await Program.ApiUrl.AppendPathSegments("query", "global", "packs")
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .PostJsonAsync(new
                {
                    query
                })
                .ReceiveJsonList();
            List<Pack> packs = new List<Pack>();
            foreach (var k in res)
            {
                var p = new Pack()
                {
                    PackName = k.name,
                    PackId = k.id,
                    PackCount = k.count
                };
                foreach(var x in k.preview)
                {
                    p.Emojis.Add(new Emoji()
                    {
                        EmojiName = x.name,
                        EmojiId = x.id
                    });
                }
                packs.Add(p);
            }

            return packs;
        }
    }
}
