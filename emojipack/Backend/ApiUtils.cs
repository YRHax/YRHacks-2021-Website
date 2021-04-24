using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace emojipack.Backend
{
    public class ApiUtils
    {
        

        public static async Task RefreshEmoji(Emoji emoji)
        {
            var user = AuthService.User;
            var pq = await Program.ApiUrl
                .AppendPathSegments("pack", emoji.EmojiId)
                .WithOAuthBearerToken(user.AccessToken)
                .GetAsync()
                .ReceiveJson();

            emoji.EmojiPackId = pq.id;
            emoji.EmojiName = pq.name;
            emoji.ClickCount = pq.count;
            emoji.EmojiPackId = pq.packid;
            emoji.EmojiOwnerId = pq.ownerid;
        }
        public static async Task RefreshPack(Pack pack)
        {
            if (Program.TESTING)
            {
                List<Emoji> emojis1 = new List<Emoji>();
                for(int i = 0; i < 100; i++)
                {
                    var emote = new Emoji()
                    {
                        ClickCount = 0,
                        EmojiName = $"e#{i}-{pack.PackName}",
                        EmojiId = Guid.NewGuid().ToString(),
                        EmojiOwnerId = AuthService.User.Id,
                        EmojiPackId = pack.PackId
                    };
                    emojis1.Add(emote);
                }

                pack.Emojis = emojis1;
                return;
            }


            var user = AuthService.User;
            var pq = await Program.ApiUrl
                .AppendPathSegments("pack", pack.PackId)
                .WithOAuthBearerToken(user.AccessToken)
                .GetAsync()
                .ReceiveJson();
            pack.Visibility = pq.visibility;
            List<Emoji> emojis = new List<Emoji>();
            foreach (var emoji in pq.emojis)
            {
                emojis.Add(new()
                {
                    ClickCount = emoji.count,
                    EmojiName = emoji.name,
                    EmojiId = emoji.id,
                    EmojiOwnerId = user.Id,
                    EmojiPackId = pack.PackId
                });
            }

            pack.PackName = pq.name;
            pack.PackOwner = user.Id;
            pack.Emojis = emojis;
        }
        public static async Task RefreshUser()
        {
            if (Program.TESTING)
            {
                List<Pack> packs1 = new List<Pack>();
                for(int i = 0; i < 100; i++)
                {
                    var pack = new Pack()
                    {
                        PackId = Guid.NewGuid().ToString(),
                        PackName = $"Pack #{i}"
                    };
                    await RefreshPack(pack);
                    packs1.Add(pack);
                }

                AuthService.User.Packs = packs1;
                return;
            }

            var user = AuthService.User;
            var res = await Program.ApiUrl
                .AppendPathSegments("query","user",user.Id)
                .WithOAuthBearerToken(user.AccessToken)
                .GetAsync()
                .ReceiveJson();
            List<Pack> packs = new List<Pack>();
            foreach (var x in res.packs)
            {
                string id = x.id, name = x.name;
                var pack = new Pack()
                {
                    PackId = id,
                    PackName = name
                };
                await RefreshPack(pack);
                packs.Add(pack);
            }

            user.Packs = packs;
        }
    }
}
