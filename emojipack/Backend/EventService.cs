using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace emojipack.Backend
{
    public class EventService
    {
        public static Emoji SelectedEmoji = null;
        public static Pack SelectedPack = null;
        public delegate void AuthLoad();
        public static event AuthLoad AuthLoadedEvent;

        public static void InvokeAuthLoad()
        {
            AuthLoadedEvent?.Invoke();
        }


        public delegate void SelectPackChanged(Pack selected);
        public static event SelectPackChanged SelectPackChangedEvent;

        public static void InvokeSelectPackChangedEvent(Pack selected)
        {
            SelectPackChangedEvent?.Invoke(selected);
        }

        public delegate void HoverEmojiChanged();
        public static event HoverEmojiChanged HoverEmojiChangedEvent;

        public static void InvokeHoverEmojiChangedEvent()
        {
            HoverEmojiChangedEvent?.Invoke();
        }
    }
}
