using PdfBuilderTool.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Utils
{
    class PDFPageReader : IPageReader2, IDisposable
    {
        public PdfReader reader { get; set; }

        public PDFPageReader(String fpin)
        {
            reader = new PdfReader(fpin);
            reader.ConsolidateNamedDestinations();
        }

        public PDFPageReader(Stream stream)
        {
            reader = new PdfReader(stream);
            reader.ConsolidateNamedDestinations();
        }

        public int NumberOfPages
        {
            get { return reader.NumberOfPages; }
        }

        public void Dispose()
        {
            reader.Close();
        }

        public Rectangle GetPageSizeWithRotation(int pageNum) => reader.GetPageSizeWithRotation(pageNum);
    }
}
