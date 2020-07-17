using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdfBuilderTool.Models
{
    public class Range
    {
        /// <summary>
        /// 1-
        /// </summary>
        public int first { get; set; }

        /// <summary>
        /// 1- ... int.MaxValue
        /// </summary>
        public int last { get; set; }

        /// <summary>
        /// l,r,d
        /// </summary>
        public string rot { get; set; }

        /// <summary>
        /// 右回転の度数
        /// 0,90,180,270 のいずれか
        /// </summary>
        public int rotAngle
        {
            get
            {
                switch (char.ToLowerInvariant((rot + " ")[0]))
                {
                    case 'l': return 270;
                    case 'r': return 90;
                    case 'd': return 180;
                }
                return 0;
            }
        }
    }
}
