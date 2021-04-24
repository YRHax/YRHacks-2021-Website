using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace emojipack.Images
{
    public class ImageConverter
    {
        public static async Task<MemoryStream> ConvertToPng(Stream input)
        {
            var img = await Image.LoadAsync(input);
            var ms = new MemoryStream();
            await img.SaveAsPngAsync(ms);
            return ms;
        }
    }
}
