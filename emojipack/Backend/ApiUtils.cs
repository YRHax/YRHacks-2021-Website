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
                try
                {
                    if (AuthService.User.Packs.Count != 0)
                    {
                        EventService.SelectedPack = AuthService.User.Packs.First();
                    }
                }
                catch
                {

                }
                return;
            }

            var user = AuthService.User;
            var res = await Program.ApiUrl
                .AppendPathSegments("users",user.Id)
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
