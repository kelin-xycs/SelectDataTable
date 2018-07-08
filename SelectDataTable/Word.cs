using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectDataTable
{
    class Word : StrSpan
    {

        public WordType type;
        
        public Word(int iLeft, int iRight, WordType type) : base(iLeft, iRight)
        {
            this.type = type;
        }
    }

    internal enum WordType
    {
        Any,
        NonWrapped,
        Wrapped,
        IncompleteWrapped
    }
    
}
