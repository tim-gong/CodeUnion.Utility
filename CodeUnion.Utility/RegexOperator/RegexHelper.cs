using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeUnion.Utility.RegexOperator
{
    public class RegexHelper
    {
        #region 正则处理Html源码

        /// <summary>
        /// 过滤html代码，将一些特殊符号转成正常符号
        /// </summary>
        /// <param name="html">html源码</param>
        /// <returns>过滤之后的html</returns>
        public static string FilterHtml(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                string[] regexs1 = { @"&(quot|#34);", @"&(amp|#38);", @"&(lt|#60);", @"&(gt|#62);", @"&(nbsp|#160);", @"&(iexcl|#161);", @"&(cent|#162);", @"&(pound|#163);", @"&(copy|#169);", @"&#(\d+);", @"-->", @"<!--.*\n" };

                string[] Replaces = { "\"", "&", "<", ">", " ", "\xa1", "\xa2", "\xa3", "\xa9", "", "\r\n", "" };

                return StrReplace(html, regexs1, Replaces);
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据正则表达式获取Hashtable集合
        /// </summary>
        /// <param name="content">需要分析的内容字符串</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="groups">Groups</param>
        /// <returns></returns>
        public static List<Hashtable> GetHashtables(string content, string pattern, string[] groups)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(pattern) && groups != null && groups.Length > 0)
            {
                MatchCollection mc = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                if (mc.Count > 0)
                {
                    List<Hashtable> strs = new List<Hashtable>();
                    foreach (Match m in mc)
                    {
                        Hashtable hashtable = new Hashtable();
                        foreach (string flag in groups)
                        {
                            hashtable.Add(flag, m.Groups[flag].Value);
                        }
                        strs.Add(hashtable);
                    }
                    mc = null;
                    return strs;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据正则表达式获取Hashtable集合
        /// </summary>
        /// <param name="content">需要分析的内容字符串</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="groups">Groups</param>
        /// <returns></returns>
        public static Hashtable GetHashtable(string content, string pattern, string[] groups)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(pattern) && groups != null && groups.Length > 0)
            {
                Match m = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    Hashtable hashtable = new Hashtable();
                    foreach (string flag in groups)
                    {
                        hashtable.Add(flag, m.Groups[flag].Value);
                    }
                    m = null;
                    return hashtable;
                }
            }
            return null;
        }

        /// <summary>
        /// 正则表达获取Html源码List集合
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="flage">正则表达式中子式名称</param>
        /// <returns>返回结果集</returns>
        public static List<string> GetContent(string content, string pattern, string flage)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(pattern))
            {
                MatchCollection mc = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                if (mc.Count > 0)
                {
                    List<string> strs = new List<string>();
                    foreach (Match m in mc)
                    {
                        strs.Add(m.Groups[flage].Value);
                    }
                    mc = null;
                    return strs;
                }
            }
            return null;
        }

        /// <summary>
        /// 正则表达式过滤网页HTML（得到特定内容）
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>返回结果集</returns>
        public static List<string> GetContent(string content, string pattern)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(pattern))
            {
                MatchCollection mc = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                if (mc.Count > 0)
                {
                    List<string> strs = new List<string>();
                    foreach (Match m in mc)
                    {
                        strs.Add(m.Value);
                    }
                    mc = null;
                    return strs;
                }
            }
            return null;
        }

        /// <summary>
        /// 正则表达式过滤网页HTML（得到特定内容）
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="flage">正则表达式中子式名称</param>
        /// <returns>返回结果</returns>
        public static string GetContentSingle(string content, string pattern, string flage)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(pattern))
            {
                Match m = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
                return m.Success ? m.Groups[flage].Value : string.Empty;
            }
            return content;
        }

        /// <summary>
        /// 正则表达式过滤网页HTML（得到特定内容）
        /// </summary>
        /// <param name="content">原始内容</param>
        /// <param name="pattern">正则表达式</param>
        /// <returns>返回结果</returns>
        public static string GetContentSingle(string content, string pattern)
        {
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(pattern))
            {
                Match m = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
                return m.Success ? m.Value : string.Empty;
            }
            return content;
        }

        /// <summary>
        /// 正则表达式 去除不符要求的字符串
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="patterns">数组：替换正则表达式规则（将要被替换字符串）</param>
        /// <returns>返回替换后的字符串</returns>
        public static string StrReplace(string str, string[] patterns)
        {
            if (!string.IsNullOrEmpty(str) && patterns.Length > 0)
            {
                foreach (string t in patterns)
                {
                    str = StrReplace(str, t, "");
                }
            }
            return str;
        }

        /// <summary>
        /// 正则表达式 字符串替换（符合正则表达式的都替换为给定内容）不区分大小写
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="pattern">替换正则表达式规则（将要被替换字符串）</param>
        /// <param name="str_rep">替换字符串</param>
        /// <returns>返回替换后的字符串</returns>
        public static string StrReplace(string str, string pattern, string str_rep)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(pattern))
            {
                Regex regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                str = regex.Replace(str, str_rep);
            }
            return str;
        }

        /// <summary>
        /// 正则表达式 替换指定次数 从第一个匹配项开始 （符合正则表达式的都替换为给定内容）不区分大小写
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="pattern">替换正则表达式规则（将要被替换字符串）</param>
        /// <param name="str_rep">替换字符串</param>
        /// <param name="count">替换次数 从第一个匹配项开始</param>
        /// <returns>返回替换后的字符串</returns>
        public static string StrReplace(string str, string pattern, string str_rep, int count)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(pattern))
            {
                Regex regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                str = regex.Replace(str, str_rep, count, 0);
            }
            return str;
        }

        /// <summary>
        /// 正则表达式 替换指定的匹配项 如果没有匹配项或是指定匹配结果超出范围 则不做任何操作 （符合正则表达式的都替换为给定内容）不区分大小写
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="pattern">替换正则表达式规则（将要被替换字符串）</param>
        /// <param name="str_rep">替换字符串</param>
        /// <param name="index">替换指定的匹配项 第一个就是 1 </param>
        /// <returns>返回替换后的字符串</returns>
        public static string StrReplaceOne(string str, string pattern, string str_rep, int index)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (!string.IsNullOrEmpty(pattern))
            {
                MatchCollection mc = Regex.Matches(str, pattern);
                if (index > 0 && mc.Count > index)
                {
                    Match m = mc[index - 1];
                    Regex regex = new Regex(m.Value, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    return regex.Replace(str, str_rep, 1, m.Index);
                }
            }
            return str;
        }

        /// <summary>
        /// 正则表达式 字符串替换（符合正则表达式的都替换为给定内容）不区分大小写
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <param name="patterns">数组：替换正则表达式规则（将要被替换字符串）</param>
        /// <param name="strReps">数组：替换字符串</param>
        /// <returns>返回替换后的字符串</returns>
        public static string StrReplace(string str, string[] patterns, string[] strReps)
        {
            if (!string.IsNullOrEmpty(str) && patterns.Length > 0 && patterns.Length.Equals(strReps.Length))
            {
                for (int i = 0; i < patterns.Length; i++)
                {
                    str = StrReplace(str, patterns[i], strReps[i]);
                }
            }
            return str;
        }

        /// <summary>
        /// 将字符串按照指定正则分割
        /// </summary>
        /// <param name="content">需要分割的字符串</param>
        /// <param name="pattern">分割正则</param>
        /// <returns></returns>
        public static string[] SplitContent(string content, string pattern)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(pattern))
            {
                return null;
            }
            return Regex.Split(content, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 将字符串按照特定分隔符分割为字符串数组
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="separator">分隔符</param>
        /// <returns>返回分割后的字符数组</returns>
        public static string[] SplitContent(string str, char separator)
        {
            if (!string.IsNullOrEmpty(str) && separator.Equals(""))
            {
                string[] strs = str.Split(separator);
                return strs;
            }
            return null;
        }


        #endregion
    }
}
