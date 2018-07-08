using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectDataTable
{
    class Parser
    {

        public readonly static char[] _whiteSpaces = { ' ', '\r', '\n' };

        public readonly static char[] _wrapperList = { '\'', '[', '(' };


        private static int FindWrapperEnd单引号(char[] sql, int beginIndex)
        {
            for (int i = beginIndex + 1; i < sql.Length; i++)
            {
                if (sql[i] == '\'')
                    return i;
            }

            return -1;
        }
        private static int FindWrapperEnd中括号(char[] sql, int beginIndex)
        {
            int j;

            for (int i = beginIndex + 1; i < sql.Length; i++)
            {
                if (sql[i] == '\'')
                {
                    j = FindWrapperEnd单引号(sql, i);

                    if (j == -1)
                        return -1;

                    i = j;
                }

                if (sql[i] == ']')
                    return i;


            }

            return -1;
        }
        private static int FindWrapperEnd小括号(char[] sql, int beginIndex, int endIndex)
        {
            int counter = 1;

            int 右单引号Index;

            for (int i = beginIndex + 1; i <= endIndex; i++)
            {
                if (sql[i] == '\'')
                {
                    右单引号Index = FindWrapperEnd单引号(sql, i);

                    if (右单引号Index == -1)
                        return -1;

                    i = 右单引号Index;
                }

                if (sql[i] == ')')
                {
                    counter--;

                    if (counter == 0)
                        return i;
                }

                if (sql[i] == '(')
                {
                    counter++;
                }

            }

            return -1;
        }

        private static int FindWrapper(char[] sql, int beginIndex, int endIndex, char[] wrapperList)
        {
            return StrUtil.FindForward(sql, beginIndex, endIndex, wrapperList);
        }

        private static int FindWrapperEnd(char[] sql, int beginIndex)
        {
            char left = sql[beginIndex];

            if (left == '\'')
            {
                return FindWrapperEnd单引号(sql, beginIndex);
            }

            else if (left == '[')
            {
                return FindWrapperEnd中括号(sql, beginIndex);
            }

            else if (left == '(')
            {
                return FindWrapperEnd小括号(sql, beginIndex, sql.Length - 1);
            }

            throw new SelectDataTableException("无效的 Left Wrapper ：\"" + left + "\" 。 ", sql, beginIndex);
        }

        public static List<Word> GetWordList(char[] sql, int beginIndex, int endIndex)
        {
            int leftIndex = beginIndex;
            int rightIndex;

            List<Word> wordList = new List<Word>();

            while (true)
            {

                rightIndex = FindWrapper(sql, leftIndex, endIndex, _wrapperList);

                if (rightIndex == -1)
                {
                    wordList.Add(new Word(leftIndex, endIndex, WordType.NonWrapped));
                    return wordList;
                }

                if (rightIndex == leftIndex)
                {
                    rightIndex = FindWrapperEnd(sql, leftIndex);

                    if (rightIndex == -1)
                    {
                        wordList.Add(new Word(leftIndex, endIndex, WordType.IncompleteWrapped));
                        return wordList;
                    }

                    wordList.Add(new Word(leftIndex, rightIndex, WordType.Wrapped));
                    leftIndex = rightIndex + 1;

                    continue;
                }

                wordList.Add(new Word(leftIndex, rightIndex - 1, WordType.NonWrapped));

                if (rightIndex > endIndex)
                    break;

                leftIndex = rightIndex;

            }

            return wordList;
        }

        public static Content GetContent(char[] sql, int beginIndex, int endIndex)
        {

            if (beginIndex > endIndex)
                throw new SelectDataTableException("beginIndex 应 小于等于 endIndex 。 beginIndex ： " + beginIndex + " endIndex ： " + endIndex + " 。 ", sql, beginIndex);


            StrSpan s = StrUtil.Trim(sql, beginIndex, endIndex, Parser._whiteSpaces);
            
            Content content;

            if ( s.isEmpty )
            {
                content = new Content(s.iLeft, s.iRight, ContentType.None);
                content.isEmpty = true;

                return content;
            }

            char cLeft = sql[s.iLeft];
            char cRight = sql[s.iRight];

            if (cLeft == '\'')
            {
                s = GetContent单引号(sql, s);

                content = new Content(s.iLeft, s.iRight, ContentType.String);

                if ( s.isEmpty )
                {
                    content.isEmpty = true;
                }

                return content;
            }
            else if (cLeft == '[')
            {
                s = GetContent中括号(sql, s);
            }
            else if (cLeft == '(')
            {
                s = GetContent小括号(sql, s);

                content = new Content(s.iLeft, s.iRight, ContentType.Express);

                if ( s.isEmpty )
                {
                    content.isEmpty = true;
                }

                return content;
            }

            if (StrUtil.IsNumber( sql[s.iLeft] ))
            {
                return new Content(s.iLeft, s.iRight, ContentType.Number);
            }
            
            return new Content(s.iLeft, s.iRight, ContentType.Column);
            
            //throw new Exception("语法错误。 在 第 " + s.iLeft + " 个 字符 \"" + sql[s.iLeft] + "\" 。");
        }

        private static StrSpan GetContent单引号(char[] sql, StrSpan s)
        {

            char cLeft = sql[s.iLeft];
            char cRight = sql[s.iRight];


            if (s.iLeft == s.iRight)
                throw new SelectDataTableException("无效的 字符串， 缺少 结束单引号 。 ", sql, s.iLeft);

            if (cRight != '\'')
                throw new SelectDataTableException("无效的 字符串， 缺少 结束单引号 。 ", sql, s.iRight);


            if (s.iRight == s.iLeft + 1)
            {
                s = new StrSpan(s.iLeft, s.iRight);
                s.isEmpty = true;

                return s;
            }

            return new StrSpan(s.iLeft + 1, s.iRight - 1);
        }

        private static StrSpan GetContent中括号(char[] sql, StrSpan s)
        {

            char cLeft = sql[s.iLeft];
            char cRight = sql[s.iRight];


            if (s.iLeft == s.iRight)
                throw new SelectDataTableException("无效的 中括号， 缺少 结束中括号 。 ", sql, s.iLeft);

            if (cRight != ']')
                throw new SelectDataTableException("无效的 中括号， 缺少 结束中括号 。 ", sql, s.iRight);


            if (s.iRight == s.iLeft + 1)
            {
                s = new StrSpan(s.iLeft, s.iRight);
                s.isEmpty = true;

                return s;
            }

            s = StrUtil.Trim(sql, s.iLeft + 1, s.iRight - 1, _whiteSpaces);

            return new StrSpan(s.iLeft, s.iRight);
        }

        private static StrSpan GetContent小括号(char[] sql, StrSpan s)
        {

            char cLeft = sql[s.iLeft];
            char cRight = sql[s.iRight];


            if (s.iLeft == s.iRight)
                throw new SelectDataTableException("无效的 小括号， 缺少 结束小括号 。 ", sql, s.iLeft);


            if (s.iRight == s.iLeft + 1)
            {
                if (cRight == ')')
                {
                    s = new StrSpan(s.iLeft, s.iRight);
                    s.isEmpty = true;

                    return s;
                }
                else
                {
                    throw new SelectDataTableException("无效的 小括号， 缺少 结束小括号 。 ", sql, s.iRight);
                }
            }

            int i = FindWrapperEnd小括号(sql, s.iLeft, s.iRight);

            if (i == -1)
                throw new SelectDataTableException("无效的 小括号， 缺少 结束小括号 。 ", sql, s.iRight);

            if (i < s.iRight)
                throw new SelectDataTableException("语法错误。存在无效的字符 。 ", sql, i + 1);


            s = StrUtil.Trim(sql, s.iLeft + 1, s.iRight - 1, _whiteSpaces);  //  去掉 小括号 并 对 内容 Trim


            return s;

            //if ( s.isEmpty )
            //{
            //    s = new StrSpan(s.iLeft, s.iRight);
            //    s.isEmpty = true;

            //    return s;
            //}


            //i = s.iLeft;
            //int j = s.iRight;

            //while (true)
            //{
            //    if (sql[i] == '(' && sql[j] == ')')
            //    {
            //        if (i + 1 == j)
            //        {
            //            s = new StrSpan(i, j);
            //            s.isEmpty = true;

            //            return s;
            //        }
            //        if (i + 1 <= j - 1)
            //        {
            //            i++;
            //            j--;
            //        }
            //    }
            //    else
            //    {
            //        break;
            //    }
            //    //return GetContent小括号(sql, s);    //  去掉 重复 括号
            //}

            //return new StrSpan(s.iLeft, s.iRight);
        }
    }

}
