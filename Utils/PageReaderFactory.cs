using PdfBuilderTool.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Utils
{
    public static class PageReaderFactory
    {
        /// <summary>
        /// Obtain IPageReader2
        /// </summary>
        /// <param name="imageFile">Seekable Stream</param>
        /// <param name="ext">`.tif`, `.tiff` or `.pdf`</param>
        /// <returns></returns>
        public static IPageReader2 Create(Stream imageFile, string ext)
        {
            ext = ext.ToLowerInvariant();

            if (ext == ".tif" || ext == ".tiff")
            {
                return new TIFPageReader(imageFile);
            }
            else if (ext == ".pdf")
            {
                return new PDFPageReader(imageFile);
            }
            else
            {
                return new WindowsBitmapReader(imageFile);
            }

            throw new NotSupportedException(ext);
        }
    }
}
