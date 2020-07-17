using PdfBuilderTool.Interfaces;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Utils
{
    public class WindowsBitmapReader : ISimpleImageProvider
    {
        Bitmap bitmap;

        public WindowsBitmapReader(Stream stream)
        {
            bitmap = new Bitmap(stream);
        }

        public int NumberOfPages => bitmap.GetFrameCount(FrameDimension.Page);

        public iTextSharp.text.Image GetImage(int pageNum)
        {
            bitmap.SelectActiveFrame(FrameDimension.Page, pageNum - 1);
            return iTextSharp.text.Image.GetInstance(bitmap, ImageFormat.Png);
        }

        public iTextSharp.text.Rectangle GetPageSizeWithRotation(int pageNum)
        {
            bitmap.SelectActiveFrame(FrameDimension.Page, pageNum - 1);

            var pw = bitmap.Width / bitmap.HorizontalResolution * 25.4f * (1191f / 420f);
            var ph = bitmap.Height / bitmap.VerticalResolution * 25.4f * (1191f / 420f);

            return new iTextSharp.text.Rectangle(0, 0, pw, ph);
        }
    }
}
