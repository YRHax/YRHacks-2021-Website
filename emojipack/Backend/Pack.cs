﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace emojipack.Backend
{
    public class Pack
    {
        public string PackOwner { get; set; }
        public string PackName { get; set; }
        public string PackId { get; set; }
        public bool Visibility { get; set; }
        public List<Emoji> Emojis { get; set; } = new List<Emoji>();
        public async Task RenamePack(string name)
        {
            var res = await Program.ApiUrl
                .AppendPathSegments("pack", "edit")
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .PatchJsonAsync(new
                {
                    id = PackId,
                    newName = name
                });
            if (res.ResponseMessage.IsSuccessStatusCode)
            {
                PackName = name;
            }
        }
        public async Task ChangeVisibility(bool visible)
        {
            var res = await Program.ApiUrl
                .AppendPathSegments("pack", "edit")
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .PatchJsonAsync(new
                {
                    id = PackId,
                    newVisibility = visible
                });
            if (res.ResponseMessage.IsSuccessStatusCode)
            {
                Visibility = visible;
            }
        }
        public async Task DeleteEmoji(Emoji emoji)
        {
            var res = await Program.ApiUrl
                .AppendPathSegments("emoji", "delete", emoji.EmojiId)
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .DeleteAsync();

            if (res.ResponseMessage.IsSuccessStatusCode)
            {
                Emojis.Remove(emoji);
            }
        }
        public async Task<Emoji> CloneEmoji(string source)
        {
            var res = await Program.ApiUrl
                .AppendPathSegments("emoji", "clone")
                .WithOAuthBearerToken(AuthService.User.AccessToken)
                .PostJsonAsync(new
                {
                    srcId = source,
                    destPackId = PackId
                });

            if (res.ResponseMessage.IsSuccessStatusCode)
            {
                var val = await res.GetJsonAsync();
                var emote = new Emoji()
                {
                    EmojiId = val.id,
                    EmojiOwnerId = PackOwner,
                    EmojiPackId = PackId
                };
                await ApiUtils.RefreshEmoji(emote);
                Emojis.Add(emote);
                return emote;
            }
            return null;
        }

        public async Task<string> GetEmojiPreviewSource()
        {
            if (Emojis.Count != 0)
            {
                var x = (from q in Emojis
                    orderby q.ClickCount descending
                    select q).First();
                return await x.GetEmojiSrc();
            }
            else
            {
                return "unknown.png";
            }
        }
    }
}
