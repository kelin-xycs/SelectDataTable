using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectDataTable.Operators
{
    abstract class Operator
    {

        public char[] sql;

        public int iLeft;
        public int iRight;

        public string str;


        public Operator(char[] sql, int iLeft, int iRight)
        {
            this.sql = sql;

            this.iLeft = iLeft;
            this.iRight = iRight;

            this.str = new string(sql, iLeft, iRight - iLeft + 1);
        }


        public abstract int Priority { get; }

        public abstract dynamic Exec(dynamic val1, dynamic val2);

        protected void CheckTypeDateTime(ref dynamic val1, ref dynamic val2)
        {
            if (val1 is DateTime && val2 is string)
            {
                val2 = DateTime.Parse(val2);
                return;
            }

            if (val1 is string && val2 is DateTime)
            {
                val1 = DateTime.Parse(val1);
                return;
            }
        }
    }

    class Add : Operator
    {
        public Add(char[] sql, int iLeft, int iRight) : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 3; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            return  val1 + val2;
        }
    }

    class Sub : Operator
    {
        public Sub(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 3; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            return val1 - val2;
        }
    }

    class Multi : Operator
    {
        public Multi(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 4; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。", sql, this.iLeft);
            //}

            return val1 * val2;
        }
    }

    class Div : Operator
    {
        public Div(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 4; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            return val1 / val2;
        }
    }

    class And : Operator
    {
        public And(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 1; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            return val1 && val2;
        }
    }

    class Or : Operator
    {
        public Or(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 0; }
        }

        public override object Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            return val1 || val2;
        }
    }

    class Not : Operator
    {
        public Not(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 5; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if ( val2 == null )
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            return   ! val2;
        }
    }

    class Equal : Operator
    {
        public Equal(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 2; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            CheckTypeDateTime(ref val1, ref val2);

            return val1 == val2;
        }
    }

    class Greater : Operator
    {
        public Greater(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 2; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            CheckTypeDateTime(ref val1, ref val2);

            return val1 > val2;
        }
    }

    class Less : Operator
    {
        public Less(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 2; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            CheckTypeDateTime(ref val1, ref val2);

            return val1 < val2;
        }
    }

    class GreaterEqual : Operator
    {
        public GreaterEqual(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 2; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            CheckTypeDateTime(ref val1, ref val2);

            return val1 >= val2;
        }
    }

    class LessEqual : Operator
    {
        public LessEqual(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 2; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            CheckTypeDateTime(ref val1, ref val2);

            return val1 <= val2;
        }
    }

    class NotEqual : Operator
    {
        public NotEqual(char[] sql, int iLeft, int iRight)
            : base(sql, iLeft, iRight)
        {

        }

        public override int Priority
        {
            get { return 2; }
        }

        public override dynamic Exec(dynamic val1, dynamic val2)
        {
            //if (val1 == null || val2 == null)
            //{
            //    throw new SelectDataTableException("缺少操作数 。 运算符 \"" + this.str + "\" 。 ", sql, this.iLeft);
            //}

            CheckTypeDateTime(ref val1, ref val2);

            return val1 != val2;
        }
    }
}
