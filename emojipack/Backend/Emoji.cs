using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace emojipack.Backend
{
    public class Emoji
    {
        public string EmojiName { get; set; }
        public string EmojiPackId { get; set; }
        public string EmojiOwnerId { get; set; }
        public int ClickCount { get; set; }
        public string EmojiId { get; set; }

        public string EmojiData { get; set; }

        public async Task RenameEmoji(string name)
        {
            if (Program.TESTING)
            {
                EmojiName = name;
                return;
            }

            var res = await Program.ApiUrl
                .AppendPathSegments("emoji", "edit")
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .PatchJsonAsync(new
                {
                    id = EmojiId,
                    newName = name
                });
            if (res.ResponseMessage.IsSuccessStatusCode)
            {
                EmojiName = name;
            }
        }
        public async Task ChangePack(Pack cur, Pack dest)
        {
            if (Program.TESTING)
            {
                dest.Emojis.Add(this);
                cur.Emojis.Remove(this);
                EmojiPackId = dest.PackId;
                return;
            }

            var res = await Program.ApiUrl
                .AppendPathSegments("emoji", "edit")
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .PatchJsonAsync(new
                {
                    id = EmojiId,
                    newPack = dest.PackId
                });
            if (res.ResponseMessage.IsSuccessStatusCode)
            {
                dest.Emojis.Add(this);
                cur.Emojis.Remove(this);
                EmojiPackId = dest.PackId;
            }
        }

        public async Task<byte[]> GetEmoji()
        {
            if (Program.TESTING)
            {
                return Convert.FromBase64String(EmojiData);
                //int r = EmojiId.GetHashCode();
                //if (r % 2 == 0)
                //{
                //    return await Program.DebugBaseAddress.AppendPathSegment("rooPog.png").GetBytesAsync();
                //}
                //else
                //{
                //    return await Program.DebugBaseAddress.AppendPathSegment("blobban.png").GetBytesAsync();
                //}
            }
            return await Program.ApiUrl
                .AppendPathSegments("emoji", "load", EmojiId)
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .GetBytesAsync();
        }

        public async Task<string> GetEmojiSrc()
        {
            return "data:image/bmp;base64," + Convert.ToBase64String(await GetEmoji());
        }
    }
}
