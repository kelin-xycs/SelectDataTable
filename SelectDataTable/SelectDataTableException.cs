using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectDataTable
{
    class SelectDataTableException : Exception
    {
        private string message;

        public SelectDataTableException(string message, char[] sql, int charIndex)
        {
            int i;
            int j;

            int iLeft = charIndex;
            int iRight = charIndex;

            while(true)
            {
                i = iLeft - 1;
                j = iRight + 1;

                if (i >= 0)
                {
                    iLeft = i;
                }

                if (j < sql.Length)
                {
                    iRight = j;
                }

                int l = iRight - iLeft + 1;

                if (l >= sql.Length || l >= 18)
                {
                    this.message = message + " " + "在 第 " + charIndex + " 个 字符 \"" 
                        + new string(sql, iLeft, iRight - iLeft + 1) + "\" 附近。";

                    break;
                }

            }
        }

        public override string Message
        {
            get
            {
                return this.message;
            }
        }
    }
}
