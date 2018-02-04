using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Extensions
{
    public static class BitmapExtensions
    {
        /// <summary>
        /// Creates a thumbnail from an existing image. Sets the biggest dimension of the
        /// thumbnail to either desiredWidth or Height and scales the other dimension down
        /// to preserve the aspect ratio
        /// </summary>
        /// <param name="originalBmp"></param>
        /// <param name="desiredWidth">maximum desired width of thumbnail</param>
        /// <param name="desiredHeight">maximum desired height of thumbnail</param>
        /// <returns>Bitmap thumbnail</returns>
        public static Bitmap CreateThumbnail(this Bitmap originalBmp, int desiredWidth, int desiredHeight)
        {
            // If the image is smaller than a thumbnail just return it
            if (originalBmp.Width <= desiredWidth && originalBmp.Height <= desiredHeight)
            {
                return originalBmp;
            }

            int newWidth, newHeight;

            // scale down the smaller dimension
            if (desiredWidth * originalBmp.Height < desiredHeight * originalBmp.Width)
            {
                newWidth = desiredWidth;
                newHeight = (int)Math.Round((decimal)originalBmp.Height * desiredWidth / originalBmp.Width);
            }
            else
            {
                newHeight = desiredHeight;
                newWidth = (int)Math.Round((decimal)originalBmp.Width * desiredHeight / originalBmp.Height);
            }

            // This code creates cleaner (though bigger) thumbnails and properly
            // and handles GIF files better by generating a white background for
            // transparent images (as opposed to black)
            // This is preferred to calling Bitmap.GetThumbnailImage()
            Bitmap bmpOut = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(bmpOut))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
                graphics.DrawImage(originalBmp, 0, 0, newWidth, newHeight);
            }

            return bmpOut;
        }

        public static Bitmap CreateThumbnail(Stream stream, int desiredWidth, int desiredHeight)
        {
            var originalBmp = new Bitmap(stream);
            return CreateThumbnail(originalBmp, desiredWidth, desiredHeight);
        }

    }

}
