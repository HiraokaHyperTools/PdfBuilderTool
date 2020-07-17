using PdfBuilderTool.Interfaces;
using PdfBuilderTool.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Utils
{
    public class PDFWriter : IDisposable
    {
        Document document;
        PdfCopy copy;
        Stream fs;
        List<PdfReader> includedReaders = new List<PdfReader>();

        public PDFWriter(String fppdf)
        {
            this.fs = File.Create(fppdf);
        }

        public PDFWriter(Stream writeTo)
        {
            this.fs = writeTo;
        }

        public void AddRanges(IPageReader2 reader, IEnumerable<Range> ranges)
        {
            var addPDF = reader as PDFPageReader;
            var addImage = reader as ISimpleImageProvider;

            foreach (var range in ranges)
            {
                for (int y = range.first; y <= range.last && y <= reader.NumberOfPages; y++)
                {
                    if (document == null)
                    {
                        document = new Document(reader.GetPageSizeWithRotation(y));
                        copy = new PdfCopy(document, fs);
                        document.Open();
                    }

                    if (addPDF != null)
                    {
                        // http://stackoverflow.com/a/6155962
                        int rot = addPDF.reader.GetPageRotation(y);
                        var pageDict = addPDF.reader.GetPageN(y);
                        pageDict.Put(PdfName.ROTATE, new PdfNumber((rot + range.rotAngle) % 360));
                        PdfImportedPage page = copy.GetImportedPage(addPDF.reader, y);
                        copy.AddPage(page);
                    }
                    else if (addImage != null)
                    {
                        iTextSharp.text.Image pic = addImage.GetImage(y);

                        Document localDoc = new Document();
                        MemoryStream localStream = new MemoryStream();
                        PdfWriter localWriter = PdfWriter.GetInstance(localDoc, new NoclosePassthru(localStream));

                        var pdfPageSize = addImage.GetPageSizeWithRotation(y);

                        localDoc.SetPageSize(pdfPageSize); // A3横
                        localDoc.SetMargins(0, 0, 0, 0);
                        localDoc.Open();

                        pic.ScaleAbsolute(pdfPageSize.Width, pdfPageSize.Height);
                        localDoc.Add(pic);

                        localDoc.Close();
                        localWriter.Close();

                        localStream.Position = 0;

                        PdfReader localReader = new PdfReader(localStream);

                        var pageDict = localReader.GetPageN(y);
                        pageDict.Put(PdfName.ROTATE, new PdfNumber(range.rotAngle % 360));

                        PdfImportedPage page = copy.GetImportedPage(localReader, 1);
                        copy.AddPage(page);

                        includedReaders.Add(localReader);
                    }
                    else
                    {
                        throw new NotSupportedException("未対応 " + reader);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (copy != null) copy.Close();
            if (document != null) document.Close();
            if (fs != null) fs.Close();
        }
    }
}
