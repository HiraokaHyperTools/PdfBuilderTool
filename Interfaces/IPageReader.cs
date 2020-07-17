using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Interfaces
{
    public interface IPageReader
    {
        int NumberOfPages { get; }
    }
}
