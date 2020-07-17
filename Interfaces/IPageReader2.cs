using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Interfaces
{
    public interface IPageReader2 : IPageReader
    {
        /// <summary>
        /// </summary>
        /// <param name="pageNum">1-</param>
        /// <returns></returns>
        Rectangle GetPageSizeWithRotation(int pageNum);
    }
}
