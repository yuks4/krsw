using System;
using System.Text.RegularExpressions;

namespace nanka
{
    /// <summary>
    /// Regular の概要の説明です。
    /// (使用例)str = regular.Replace(str, @"ove(rr)ide.{3}", @"_[$1]_");
    /// </summary>
    public class StringRegular
    {
        public StringRegular()
        {
        }
        /// <summary>
        /// 入力文字列の最初の文字から検索を開始し、正規表現によって定義されている文字パターンに一致するすべての対象を置換文字列で置き換えます。
        /// </summary>
        /// <param name="baseStr">変更対象の文字列</param>
        /// <param name="patternStr">一致させる正規表現パターン</param>
        /// <param name="replaceStr">置換文字列</param>
        /// <returns>変更後の文字列</returns>
        public string Replace(string baseStr, string patternStr, string replaceStr)
        {
            if (null == baseStr) return "";
            if (null == patternStr) return baseStr;
            if (null == replaceStr) return replaceStr = "";
            try
            {
                return Regex.Replace(baseStr, patternStr, replaceStr);
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 入力文字列の最初の文字から検索を開始し、正規表現によって定義されている文字パターンに一致するすべての対象を置換文字列で置き換えます。
        /// </summary>
        /// <param name="baseStr[]">変更対象の文字列</param>
        /// <param name="patternStr">一致させる正規表現パターン</param>
        /// <param name="replaceStr">置換文字列</param>
        /// <returns>変更後の文字列</returns>
        public string[] Replace(string[] baseStr, string patternStr, string replaceStr)
        {
            string[] outStr = new string[baseStr.Length];
            for (int i = 0; i < baseStr.Length; i++)
            {
                outStr[i] = Replace(baseStr[i], patternStr, replaceStr);
            }
            return outStr;
        }
        /// <summary>
        /// 入力文字列の最初の文字から検索を開始し、正規表現によって定義されている文字パターンに一致するすべての対象を置換文字列で置き換えます。
        /// </summary>
        /// <param name="baseStr[]">変更対象の文字列</param>
        /// <param name="patternStr[]">一致させる正規表現パターン</param>
        /// <param name="replaceStr[]">置換文字列</param>
        /// <returns>変更後の文字列</returns>
        public string[] Replace(string[] baseStr, string[] patternStr, string[] replaceStr)
        {
            return Replace(baseStr, patternStr[0], replaceStr[0]);
        }
        /// <summary>
        /// 入力文字列の最初の文字から検索を開始し、正規表現によって定義されている文字パターンに一致するか調べる。
        /// </summary>
        /// <param name="baseStr">対象の文字列</param>
        /// <param name="patternStr">一致させる正規表現パターン</param>
        /// <returns>一致すればtrue</returns>
        public bool Match(string baseStr, string patternStr)
        {
            if (null == baseStr || null == patternStr) return false;
            try
            {

                return (System.Text.RegularExpressions.Match.Empty
                    != Regex.Match(baseStr, patternStr)) ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
