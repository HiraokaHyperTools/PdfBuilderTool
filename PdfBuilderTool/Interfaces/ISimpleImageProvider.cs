using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Interfaces
{
    public interface ISimpleImageProvider : IPageReader2
    {
        /// <summary>
        /// </summary>
        /// <param name="pageNum">1-</param>
        /// <returns></returns>
        iTextSharp.text.Image GetImage(int pageNum);
    }
}
