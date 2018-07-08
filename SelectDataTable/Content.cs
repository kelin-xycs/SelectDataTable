using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectDataTable
{
    class Content : StrSpan
    {

        public ContentType type;

        public Content(int iLeft, int iRight, ContentType type) : base(iLeft, iRight)
        {
            this.type = type;
        }
    }

    enum ContentType
    {
        None,
        String,
        Number,
        Column,
        Express
    }
}
