using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using SelectDataTable.Operators;

namespace SelectDataTable
{
    abstract class Express
    {

        public abstract dynamic Exec(DataRow dr);

    }

    class OperatorExpress : Express
    {
        private Operator op;
        private Express express1;
        private Express express2;

        public OperatorExpress(Operator op, Express express1, Express express2)
        {
            this.op = op;
            this.express1 = express1;
            this.express2 = express2;

            if (op is Not)
            {
                if (express2 == null)
                {
                    throw new SelectDataTableException
                        ("缺少表达式 。 运算符 \"" + this.op.str + "\" 。 ", this.op.sql, this.op.iLeft);
                }

                if (express1 != null)
                {
                    throw new SelectDataTableException
                        ("not 运算符之前不应有表达式 。 运算符 \"" + this.op.str + "\" 。 ", this.op.sql, this.op.iLeft);
                }
            }
            else
            {
                if (express1 == null || express2 == null)
                {
                    throw new SelectDataTableException
                        ("缺少表达式 。 运算符 \"" + this.op.str + "\" 。 ", this.op.sql, this.op.iLeft);
                }
            }
        }

        public override dynamic Exec(DataRow dr)
        {
            return op.Exec(
                this.express1 == null ? null : this.express1.Exec(dr), 
                this.express2 == null ? null : this.express2.Exec(dr)
                );
        }
    }

    class StringExpress : Express
    {

        private string str;

        public StringExpress(char[] sql, Content content)
        {
            str = new string(sql, content.iLeft, content.iRight - content.iLeft + 1);
        }

        public override dynamic Exec(DataRow dr)
        {
            return str;
        }
    }

    class NumberExpress : Express
    {
        private object num;

        public NumberExpress(char[] sql, Content content)
        {
            string s = new string(sql, content.iLeft, content.iRight - content.iLeft + 1);

            int i;
            long l;
            float f;
            double d;
            decimal de;

            if ( int.TryParse(s, out i) )
            {
                num = i;
            }
            else if ( long.TryParse(s, out l) )
            {
                num = l;
            }
            else if ( float.TryParse(s, out f) )
            {
                num = f;
            }
            else if ( double.TryParse(s, out d) )
            {
                num = d;
            }
            else if ( decimal.TryParse(s, out de) )
            {
                num = de;
            }
            else
            {
                throw new SelectDataTableException("无效的数字格式 \"" + s + "\" 。 ", sql, content.iLeft);
            }

        }

        public override dynamic Exec(DataRow dr)
        {
            return num;
        }


    }

    class ColumnExpress : Express
    {

        private string columnName;

        public ColumnExpress(char[] sql, Content content)
        {
            columnName = new string(sql, content.iLeft, content.iRight - content.iLeft + 1);
        }

        public override dynamic Exec(DataRow dr)
        {
            return dr[ columnName ];
        }
    }
}
