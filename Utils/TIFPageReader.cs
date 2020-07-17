using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PdfBuilderTool.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec;

namespace PdfBuilderTool.Utils
{
    public class TIFPageReader : ISimpleImageProvider
    {
        private readonly RandomAccessFileOrArray ram;
        private readonly Stream si;
        private readonly CTIF ctif;

        public TIFPageReader(Stream si)
        {
            this.si = si;

            ctif = new CTIF();
            ctif.Read(si);

            si.Position = 0;

            ram = new RandomAccessFileOrArray(si);
        }

        public int NumberOfPages => ctif.Pages.Count();

        public Image GetImage(int pageNum)
        {
            try
            {
                return TiffImage.GetTiffImage(ram, pageNum);
            }
            catch (IOException ex)
            {
                // System.IO.IOException: Compression JPEG is only supported with a single strip. This image has 30 strips.
                if (ex.Message.StartsWith("Compression JPEG is only supported with a single strip"))
                {
                    return new WindowsBitmapReader(new NoclosePassthru(si)).GetImage(pageNum);
                }
                throw;
            }
            catch (ArgumentException ex)
            {
                // System.ArgumentException: Extra samples are not supported.
                if (ex.Message.StartsWith("Extra samples are not supported"))
                {
                    return new WindowsBitmapReader(new NoclosePassthru(si)).GetImage(pageNum);
                }
                throw;
            }
        }

        public Rectangle GetPageSizeWithRotation(int pageNum)
        {
            var c1 = ctif.Pages[pageNum - 1];

            var pw = c1.Width / c1.HorizontalResolution * 25.4f * (1191f / 420f);
            var ph = c1.Height / c1.VerticalResolution * 25.4f * (1191f / 420f);

            return new Rectangle(0, 0, pw, ph);
        }
    }
}
