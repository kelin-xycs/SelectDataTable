using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectDataTable
{
    internal class SqlParser
    {




        public static void Parse(string sqlStr, out List<string> columnList, out Express whereExpress)
        {


            char[] sql = sqlStr.ToLower().ToCharArray();


            List<Word> wordList = Parser.GetWordList(sql, 0, sql.Length - 1);

            int wordIndex = 0;

            Word wSelect = GetKeyword(sql, 0, wordIndex, wordList, "select", out wordIndex, true);

            Word wFrom = GetKeyword(sql, wSelect.iRight + 1, wordIndex, wordList, "from", out wordIndex, true);

            Word wDt = GetKeyword(sql, wFrom.iRight + 1, wordIndex, wordList, "dt", out wordIndex, true);

            Word wWhere = GetKeyword(sql, wDt.iRight + 1, wordIndex, wordList, "where", out wordIndex, false);



            Word wColumns = new Word(wSelect.iRight + 1, wFrom.iLeft - 1, WordType.Any);

           



            columnList = GetColumnList(sql, wColumns);
            //Express whereExpress = ParseExpress(wWhereCondition);

            if (wWhere != null && wWhere.iRight < sql.Length - 1)
            {
                whereExpress = ExpressParser.Parse(sql, wWhere.iRight + 1, sql.Length - 1);
            }
            else
            {
                whereExpress = null;
            }
            //Word wWhereCondition = new Word(wWhere.iRight + 1, sql.Length - 1, WordType.Any);
            


            //T.WriteLine(express.Exec());

            //T.WriteLine(columnList);






        }

        internal static List<string> GetColumnList(char[] sql, Word wColumns)
        {

            List<string> columnList = new List<string>();

            List<StrSpan> tokenList = StrUtil.Split(sql, wColumns.iLeft, wColumns.iRight,  ',');

            char cLeft;
            char cRight;

            StrSpan s;

            for(int i = 0; i < tokenList.Count ; i++)
            {
                s = tokenList[i];

                if ( s.isEmpty )
                {
                    throw new SelectDataTableException("列名 不能 为空 。 ", sql, s.iLeft);
                }
                
                s = StrUtil.Trim(sql, s.iLeft, s.iRight, Parser._whiteSpaces);

                if ( s.isEmpty )
                {
                    throw new SelectDataTableException("列名 不能 为空 。 ", sql, s.iLeft);
                }

                cLeft = sql[s.iLeft];

                if ( s.iLeft == s.iRight && ( cLeft == '[' || cLeft == ']' ) )
                {
                    throw new SelectDataTableException("无效的 列名 \"" + cLeft + "\" 。 ", sql, s.iLeft);
                }

                cRight = sql[s.iRight];

                if (cLeft == '[' && cRight == ']')
                {
                    if (s.iLeft == s.iRight - 1)
                    {
                        throw new SelectDataTableException("列名 不能 为空 。 ", sql, s.iLeft);
                    }


                    s = StrUtil.Trim(sql, s.iLeft + 1, s.iRight - 1, Parser._whiteSpaces);
                    
                    if ( s.isEmpty )
                    {
                        throw new SelectDataTableException("列名 不能 为空 。 ", sql, s.iLeft);
                    }

                    columnList.Add(new string(sql, s.iLeft, s.iRight - s.iLeft + 1));
                }
                else if (cLeft == '[' && cRight != ']')
                {
                    throw new SelectDataTableException("中括号 [] 未成对出现 。 ", sql, s.iLeft);
                }
                else if (cLeft != '[' && cRight == ']')
                {
                    throw new SelectDataTableException("中括号 [] 未成对出现 。 ", sql, s.iRight);
                }
                else
                {
                    s = StrUtil.Trim(sql, s.iLeft, s.iRight, Parser._whiteSpaces);

                    columnList.Add(new string(sql, s.iLeft, s.iRight - s.iLeft + 1));
                }
            }

            return columnList;
        }

        internal static Word GetKeyword(char[] sql, int beginIndex, int beginWordIndex, List<Word> wordList, 
            string keyword, out int wordIndex, bool throwException)
        {
            
            //Word word = wordList[0];
            //if ( word.Type != WordType.NonWrapped )
            //{
            //    throw new Exception("Sql 语句 应以 select 关键字 开始。");
            //}

            int i;
            int j;

            int iLeft = -1;
            int iRight = 0;

            Word w;

            wordIndex = beginWordIndex;

            for( i = beginWordIndex; i < wordList.Count; i++ )
            {
                wordIndex = i;

                w = wordList[i];

                if (w.type != WordType.NonWrapped)
                    continue;

                if (beginIndex > w.iRight)
                    continue;

                j = beginIndex < w.iLeft ? w.iLeft : beginIndex;

                iLeft = StrUtil.FindForward(sql, j, w.iRight,   keyword);
                    
                if (iLeft != -1)
                {

                    iRight = iLeft + keyword.Length - 1;

                    //    判断 关键字 的 两边 是否是 空格， 关键字 的 两边 应该是 空格 才是 合法的 关键字
                    //    但是， 如果 两边 和 包裹符号 () [] ''  相邻 的 话 ， 也是 合法 的 关键字
                    //    因为  GetWordList()  是 按 是否 被 () [] ''  包裹 来 划分  Word 块 的
                    //    所以 如果 关键字 的 最左边 的 字符 就是 Word 块 的 最左边 字符 的话， 这是合法的，
                    //    因为 这样 要么 是 sql 字符串 的 首字符（index == 0），
                    //    要么 就是 左边相邻 的 Word 块 是 一个 被 () [] '' 包裹的 Word 块 （w.Type == WordType.Wrapped）
                    //    右边 也 同样 。
                    if (
                         (
                            iLeft == w.iLeft 
                                || 
                            (iLeft > 0 && StrUtil.IsOneOf(sql[iLeft - 1], Parser._whiteSpaces)) 
                         )
                        &&
                         (
                            iRight == w.iRight 
                                || 
                            ((iRight < sql.Length - 1) && StrUtil.IsOneOf(sql[iRight + 1], Parser._whiteSpaces))
                         )
                      )
                    {
                        break;
                    }
                    
                }

            }

            
            //if (iLeft == -1)
            //    throw new Exception("找不到合法的 关键字 \"" + keyword + "\"。 注意 关键字 不能包含在 () [] '' 中。");

            if (iLeft == -1)
            {
                if ( throwException )
                    throw new Exception("找不到合法的 关键字 \"" + keyword + "\"。 注意 关键字 不能包含在 () [] '' 中。");
                else
                    return null;
            }
                

            return new Word(iLeft, iRight,  WordType.NonWrapped);            

        }

        
    }

    
}
