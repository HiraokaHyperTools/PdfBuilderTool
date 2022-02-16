using iTextSharp.text.pdf;
using NUnit.Framework;
using PdfBuilderTool.Utils;
using System;
using System.IO;

namespace PdfBuilderTool.Test
{
    public class Test1
    {
        [Test]
        public void Combine()
        {
            using (var file1 = File.OpenRead(Locate("Samples/1.pdf")))
            using (var file2 = File.OpenRead(Locate("Samples/2.pdf")))
            using (var file3 = File.OpenRead(Locate("Samples/3.pdf")))
            {
                var reader1 = PageReaderFactory.Create(file1, ".pdf");
                var reader2 = PageReaderFactory.Create(file2, ".pdf");
                var reader3 = PageReaderFactory.Create(file3, ".pdf");

                var outSt = new MemoryStream();

                using (var writer = new PDFWriter(outSt, leaveOpen: true))
                {
                    writer.AddAllPagesFrom(reader1, reader2, reader3);
                }

                outSt.Position = 0;

                var reader = PageReaderFactory.Create(outSt, ".pdf");
                Assert.AreEqual(1 + 2 + 3, reader.NumberOfPages);
            }
        }

        [Test]
        public void DocInfoInheritance()
        {
            using (var file1 = File.OpenRead(Locate("Samples/DocInfo.pdf")))
            {
                var outSt = new MemoryStream();
                {
                    var reader1 = PageReaderFactory.Create(file1, ".pdf");

                    using (var writer = new PDFWriter(outSt, leaveOpen: true))
                    {
                        writer.AddAllPagesFrom(reader1);
                    }
                }

                outSt.Position = 0;

                var reader = new PdfReader(outSt);
                Assert.AreEqual(1, reader.NumberOfPages);
                Assert.AreEqual("サブタイトル", "" + reader.Info["Subject"]);
                Assert.AreEqual("作成者", "" + reader.Info["Author"]);
                Assert.AreEqual("キーワード", "" + reader.Info["Keywords"]);
                Assert.AreEqual("タイトル", "" + reader.Info["Title"]);
            }
        }

        private string Locate(string path)
        {
            return Path.Combine(TestContext.CurrentContext.WorkDirectory, path);
        }
    }
}
