using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SelectDataTable.Operators;

namespace SelectDataTable
{
    class ExpressParser
    {

        

        public static Express Parse(char[] sql, int beginIndex, int endIndex)
        {

            List<Word> wordList = Parser.GetWordList(sql, beginIndex, endIndex);

            List<Word> opList = new List<Word>();


            Operator op1 = null;
            Operator op2 = null;


            foreach(Word w in wordList)
            {
                if (w.type != WordType.NonWrapped)
                    continue;

                if (op1 == null)
                {
                    op1 = GetOperator(sql, w.iLeft, w.iRight);
                }
                else
                {
                    op2 = GetOperator(sql, w.iLeft, w.iRight);

                    if (op2 != null && op2.Priority <= op1.Priority)
                    {
                        op1 = op2;
                    }
                }
            }


            if (op1 == null)
            {
                //StrSpan s = StrUtil.Trim(sql, beginIndex, endIndex, Parser._whiteSpaces);

                //if ( s.isEmpty )
                //{
                //    throw new Exception()
                //}

                Content content = Parser.GetContent(sql, beginIndex, endIndex);

                if ( content.isEmpty )
                {
                    return null;
                    //throw new Exception("语法错误， 表达式 不能为空 。 在 第 " + content.iLeft + " 个字符 \"" + sql[content.iLeft] + "\" 。");
                }

                if ( content.type == ContentType.String)
                {
                    return new StringExpress(sql, content);
                }

                if (content.type == ContentType.Number)
                {
                    return new NumberExpress(sql, content);
                }

                if (content.type == ContentType.Column)
                {
                    return new ColumnExpress(sql, content);
                }

                return Parse(sql, content.iLeft, content.iRight);
                //throw new Exception("缺少 运算符 。在 第 " + word.iLeft + " 个字符 \"" + sql[word.iLeft] + "\" 和 第 " + word.iLeft + " 个字符 \"" + sql[word.iRight] + "\" 之间 。");
            }


            Express express1 = null;
            Express express2 = null;

            if (op1.iLeft > beginIndex)
            {
                express1 = Parse(sql, beginIndex, op1.iLeft - 1);
            }

            if (op1.iRight < endIndex)
            {
                express2 = Parse(sql, op1.iRight + 1, endIndex);
            }

            Express express = new OperatorExpress(op1, express1, express2);

            return express;
        }

        private static Operator GetOperator(char[] sql, int beginIndex, int endIndex)
        {
            char c;

            Operator op1 = null;
            Operator op2 = null;

            for (int i = beginIndex; i < endIndex; )
            {
                c = sql[i];

                if (c == '+')
                {
                    op2 = new Add(sql, i, i);
                    i++;
                }
                else if (c == '-')
                {
                    op2 = new Sub(sql, i, i);
                    i++;
                }
                else if (c == '*')
                {
                    op2 = new Multi(sql, i, i);
                    i++;
                }
                else if (c == '/')
                {
                    op2 = new Div(sql, i, i);
                    i++;
                }
                else if (
                    (i + 2 <= endIndex)
                        &&
                    (c == 'a' && sql[i + 1] == 'n' && sql[i + 2] == 'd')
                        &&
                    (i == beginIndex || StrUtil.IsOneOf(sql[i - 1], Parser._whiteSpaces))
                        &&
                    (i + 2 == endIndex || i + 3 <= endIndex && StrUtil.IsOneOf(sql[i + 3], Parser._whiteSpaces))
                        )
                        {
                            op2 = new And(sql, i, i + 2);
                            i = i + 3;
                        }
                else if (
                    (i + 1 <= endIndex)
                        &&
                    (c == 'o' && sql[i + 1] == 'r')
                        &&
                    (i == beginIndex || StrUtil.IsOneOf(sql[i - 1], Parser._whiteSpaces))
                        &&
                    (i + 1 == endIndex || i + 2 <= endIndex && StrUtil.IsOneOf(sql[i + 2], Parser._whiteSpaces))
                        )
                        {
                            op2 = new Or(sql, i, i + 1);
                            i = i + 2;
                        }
                else if (
                    (i + 2 <= endIndex)
                        &&
                    (c == 'n' && sql[i + 1] == 'o' && sql[i + 2] == 't')
                        &&
                    (i == beginIndex || StrUtil.IsOneOf(sql[i - 1], Parser._whiteSpaces))
                        &&
                    (i + 2 == endIndex || i + 3 <= endIndex && StrUtil.IsOneOf(sql[i + 3], Parser._whiteSpaces))
                        )
                        {
                            op2 = new Not(sql, i, i + 2);
                            i = i + 3;
                        }
                else if (
                    (i + 1 <= endIndex)
                        &&
                    (c == '>' && sql[i + 1] == '=')
                        )
                        {
                            op2 = new GreaterEqual(sql, i, i + 1);
                            i = i + 2;
                        }
                else if (
                    (i + 1 <= endIndex)
                        &&
                    (c == '<' && sql[i + 1] == '=')
                        )
                        {
                            op2 = new LessEqual(sql, i, i + 1);
                            i = i + 2;
                        }
                else if (
                    (i + 1 <= endIndex)
                        &&
                    (c == '!' && sql[i + 1] == '=')
                        )
                        {
                            op2 = new NotEqual(sql, i, i + 1);
                            i = i + 2;
                        }
                else if (c == '=')
                {
                    op2 = new Equal(sql, i, i);
                    i++;
                }
                else if (c == '>')
                {
                    op2 = new Greater(sql, i, i);
                    i++;
                }
                else if (c == '<')
                {
                    op2 = new Less(sql, i, i);
                    i++;
                }
                else
                {
                    i++;
                }



                if (op1 == null)
                {
                    if (op2 != null)
                        op1 = op2;
                }
                //  对于 Priority（优先级） 相同 的 运算符，取 最左边 的 ,  这是 为了 适应  Not(非) 运算符 的 特性
                //  Not 运算符 是 一元运算符 。 其它 运算符 都是 二元运算符 ， 从左往右 结合 和 从右往左 结合 都一样
                //  但 Not 运算符 只能 从左往右 结合， 因为 从右往左 结合 会 找不到 操作数
                //  从 编程 的 角度 来 看， 从左往右 的 思路 也 比较 顺
                //  但 等价性 上来看， 我们 人类 的 计算习惯 是 从左到右， 而 这个 计算顺序 对应在 计算机 里 是 从右往左 结合运算符
                //  计算机 从右往左 结合运算符 之后 执行计算 的 顺序 就是 从左到右 
                //  而 从左往右 结合运算符 的 执行计算顺序 是 从右到左
                //  但 不管 计算顺序 是 从左到右 还是 从右到左， 结果 都一样
                //  综上， 从 编程 的 角度 考虑， 我们选择 了 从左到右 结合 运算符， 执行计算 的 顺序 就是 从右到左
                else if (op2 != null && op2.Priority < op1.Priority)  
                {
                    op1 = op2;
                }

                //if (c == '+')
                //    return new Add(sql, i, i);
                //else if (c == '-')
                //    return new Sub(sql, i, i);
                //else if (c == '*')
                //    return new Multi(sql, i, i);
                //else if (c == '/')
                //    return new Div(sql, i, i);
                //else if (c == 'a' && sql[i + 1] == 'n' && sql[i + 2] == 'd')
                //    return new And(sql, i, i + 2);
                //else if (c == 'o' && sql[i + 1] == 'r')
                //    return new Or(sql, i, i + 1);
                //else if (c == 'n' && sql[i + 1] == 'o' && sql[i + 1] == 't')
                //    return new Not(sql, i, i + 2);
                //else if (c == '=')
                //    return new Equal(sql, i, i + 1);
                //else if (c == '>')
                //    return new Greater(sql, i, i);
                //else if (c == '<')
                //    return new Less(sql, i, i);
                //else if (c == '>' && sql[i + 1] == '=')
                //    return new GreaterEqual(sql, i, i + 1);
                //else if (c == '<' && sql[i + 1] == '=')
                //    return new LessEqual(sql, i, i + 1);
                //else if (c == '!' && sql[i + 1] == '=')
                //    return new NotEqual(sql, i, i + 1);

            }

            return op1;
            //return null;
        }
        
    }
}
