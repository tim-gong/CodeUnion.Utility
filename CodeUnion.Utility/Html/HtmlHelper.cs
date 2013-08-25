using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using CodeUnion.Utility.RegexOperator;

namespace CodeUnion.Utility.Html
{
    public class HtmlHelper
    {
        static HtmlHelper()
        {
            Proxy = null;
        }

        /// <summary>
        /// 默认不使用任何代理null  微软默认使用ie代理
        /// </summary>
        public static WebProxy Proxy { get; set; }

        #region 文章格式化

        /// <summary>
        /// List集合去重
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Unique<T>(List<T> list) where T : class
        {
            if (list == null) return null;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                for (int j = list.Count - 1; j > i; j--)
                {
                    if (list[i] == null && list[j] == null || list[i].Equals(list[j]))
                    {
                        list.RemoveAt(j);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 文章格式化
        /// </summary>
        /// <param name="content">文章原始内容</param>
        /// <param name="htmlinclude">保留标签 [a|b|p|br]</param>
        /// <param name="replacebr">是否将BR标签换成P标签</param>
        /// <param name="isimgcenter">是否图片居中显示</param>
        /// <param name="isindent">段落是否首行缩进</param>
        /// <returns></returns>
        public static string FormatteArticle(string content, string htmlinclude, bool replacebr, bool isimgcenter, bool isindent)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            content = System.Web.HttpUtility.HtmlDecode(content);//将HTML元字符转为正常可视HTML字符
            //过滤HTML注释
            content = RegexHelper.StrReplace(content, @"<!--.*?-->", "");//过滤HTML注释
            content = RegexHelper.StrReplace(content, @"<\s*script[^>]*>[\s\S]*?</script>", "");//过来JavaScript
            //过滤所有空格[包含&nbsp;]
            //string pattern = @"[\r\n　]*\s*";
            content = content.Replace("&nbsp;", "");
            //content = StrReplace(content, pattern, "");
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            int lastIndexOf = content.LastIndexOf("</P>");
            if (lastIndexOf.Equals(-1))
            {
                lastIndexOf = content.LastIndexOf("</p>");
            }
            if (!lastIndexOf.Equals(-1))
            {
                string content1 = content.Substring(0, lastIndexOf + 1);
                string content2 = content.Substring(lastIndexOf + 2);
                content = content1 + content2;
            }
            //将div标签和h1~h7标签替换成P标签
            string pattern = @"<(div|h\d)\s*[^>]*>";
            content = RegexHelper.StrReplace(content, pattern, "<p>");
            //br标签转换成 P标签
            string pattern2 = @"<\s*br[^>]*>";
            if (replacebr)
            {
                content = RegexHelper.StrReplace(content, pattern2, "</p><p>");
            }
            //标签过滤
            if (string.IsNullOrEmpty(htmlinclude))
            {
                htmlinclude = "P|IMG|A|B|STRONG|BR";
            }
            else
            {
                if (replacebr && htmlinclude.ToLower().IndexOf("br").Equals(-1))
                {
                    htmlinclude += "|BR";
                }
            }
            pattern2 = @"<(?!/?(" + htmlinclude + @"))[^>]*/?>";
            content = RegexHelper.StrReplace(content, pattern2, "");
            //过滤所有param标签
            pattern = @"</?param[^>]+/?>";
            content = RegexHelper.StrReplace(content, pattern, "").Replace("\r\n", "");
            //过滤所有空<p></p>
            pattern = @"<p>[\r\n　]*\s*</p>";
            content = RegexHelper.StrReplace(content, pattern, "");
            //过滤所有</p>标签
            pattern = @"<\s*/\s*p\s*>";
            content = RegexHelper.StrReplace(content, pattern, "");
            pattern = @"<p\s*[^>]*>";
            string[] contents = RegexHelper.SplitContent(content, pattern);

            if (contents != null && contents.Length > 0)
            {
                string result = "";
                string str1 = "<p>";
                if (isindent)
                {
                    str1 = "<p>　　";
                }
                foreach (var str in contents)
                {
                    if (!string.IsNullOrEmpty(str.Trim()))
                    {
                        result += str1 + str.Trim() + "</p>";
                    }
                }
                if (isimgcenter)
                {
                    List<string> imgs = RegexHelper.GetContent(result, @"<\s*img[^>]*/?>");
                    if (imgs != null && imgs.Count > 0)
                    {
                        imgs = Unique(imgs);
                        foreach (var img in imgs)
                        {
                            result = result.Replace(img, "<CENTER>" + img + "</CENTER>");
                        }
                    }
                }
                result = RegexHelper.StrReplace(result, @"<\s*STRONG[^>]*>", "<STRONG>");
                result = RegexHelper.StrReplace(result, @"<\s*B[^>]*>", "<B>");
                return result;
            }
            return string.Empty;
        }

        #endregion

        #region Cookie处理 网页流处理
        /// <summary>
        /// 网页cookie拼装处理
        /// </summary>
        /// <param name="cookiein">网页提交前得cookie</param>
        /// <param name="cookieout">网页提交后的cookie</param>
        /// <returns></returns>
        public static string CookiesProcess(string cookiein, string cookieout)
        {
            if (string.IsNullOrEmpty(cookiein) && string.IsNullOrEmpty(cookieout))
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(cookiein))
            {
                return cookieout;
            }
            if (string.IsNullOrEmpty(cookieout))
            {
                return cookiein;
            }
            cookiein = cookiein.Replace(";,", ";");
            cookieout = cookieout.Replace(";,", ";");
            Hashtable hashtable = new Hashtable();
            string[] cok = cookiein.Split(';');
            foreach (string str in cok)
            {
                if (str.ToLower().Trim().StartsWith("expires") || str.ToLower().Trim().StartsWith("path") || str.ToLower().Trim().StartsWith("domain") || str.Trim().EndsWith("="))
                {
                    continue;
                }
                //if (str.ToLower().Trim().StartsWith("domain"))
                //{
                //    if (str.IndexOf(",") > 0)
                //    {
                //        string str1 = str.Substring(str.IndexOf(",") + 1);
                //        if (str1.IndexOf("=") > 0)
                //        {
                //            if (!hashtable.ContainsKey(str1.Substring(0, str1.IndexOf("="))))
                //            {
                //                hashtable.Add(str1.Substring(0, str1.IndexOf("=")), str1.Substring(str1.IndexOf("=") + 1));
                //            }
                //        }
                //    }
                //    continue;
                //}
                if (str.IndexOf("=") > 0)
                {
                    if (!hashtable.ContainsKey(str.Substring(0, str.IndexOf("="))))
                    {
                        hashtable.Add(str.Substring(0, str.IndexOf("=")), str.Substring(str.IndexOf("=") + 1));
                    }
                }
            }
            cok = cookieout.Split(';');
            foreach (string str in cok)
            {
                if (str.ToLower().Trim().StartsWith("expires") || str.ToLower().Trim().StartsWith("path") || str.ToLower().Trim().StartsWith("domain") || str.Trim().EndsWith("="))
                {
                    continue;
                }
                //if (str.Trim().StartsWith("domain"))
                //{
                //    if (str.IndexOf(",") > 0)
                //    {
                //        string str1 = str.Substring(str.IndexOf(",") + 1);
                //        if (str1.IndexOf("=") > 0)
                //        {
                //            if (!hashtable.ContainsKey(str1.Substring(0, str1.IndexOf("="))))
                //            {
                //                hashtable.Add(str1.Substring(0, str1.IndexOf("=")), str1.Substring(str1.IndexOf("=") + 1));
                //            }
                //        }
                //    }
                //    continue;
                //}
                if (str.IndexOf("=") > 0)
                {
                    if (!hashtable.ContainsKey(str.Substring(0, str.IndexOf("="))))
                    {
                        hashtable.Add(str.Substring(0, str.IndexOf("=")), str.Substring(str.IndexOf("=") + 1));
                    }
                    else
                    {
                        hashtable[str.Substring(0, str.IndexOf("="))] = str.Substring(str.IndexOf("=") + 1);
                    }
                }
            }
            cok = null;
            string cookies = string.Empty;
            foreach (DictionaryEntry dic in hashtable)
            {
                cookies += dic.Key + "=" + dic.Value + ";";
            }
            if (cookies.EndsWith(";"))
            {
                cookies = cookies.Substring(0, cookies.Length - 1);
            }
            return cookies;
        }

        /// <summary>
        /// 根据指定的编码方式 将网页文件流 转为字符串
        /// </summary>
        /// <param name="response">网页文件流</param>
        /// <param name="encoding">编码方式 可为空/null 采用默认编码</param>
        /// <returns></returns>
        public static string StreamToString(HttpWebResponse response, Encoding encoding)
        {
            if (response == null)
            {
                return string.Empty;
            }
            byte[] data;
            int ContentLength = (int)response.ContentLength;
            string charset = response.ContentEncoding;
            const int c = 1024 * 10;//一次加载10K到内存
            Stream s = response.GetResponseStream();
            if (ContentLength < 0)
            {// 不能获取数据的长度
                data = new byte[c];
                MemoryStream ms = new MemoryStream();
                if (s != null)
                {
                    int l = s.Read(data, 0, c);
                    while (l > 0)
                    {
                        ms.Write(data, 0, l);
                        l = s.Read(data, 0, c);
                    }
                }
                data = ms.ToArray();
                ms.Close();
                ms = null;
            }
            else                                                            // 数据长度已知
            {
                data = new byte[ContentLength];
                int pos = 0;
                while (ContentLength > 0)
                {
                    if (s != null)
                    {
                        int l = s.Read(data, pos, ContentLength);
                        pos += l;
                        ContentLength -= l;
                    }
                }
            }
            if (s != null) s.Close();
            s = null;
            response.Close();
            if (charset == "gzip")
            {// 若数据是压缩格式，则要进行解压
                charset = string.Empty;
                MemoryStream js = new MemoryStream();                       // 解压后的流   
                MemoryStream ms = new MemoryStream(data);                   // 用于解压的流   
                GZipStream g = new GZipStream(ms, CompressionMode.Decompress);
                byte[] buffer = new byte[c];                                // 读数据缓冲区      
                int l = g.Read(buffer, 0, c);                               // 一次读 10K      
                while (l > 0)
                {
                    js.Write(buffer, 0, l);
                    l = g.Read(buffer, 0, c);
                }
                g.Close();
                ms.Close();
                g = null;
                ms = null;
                data = js.ToArray();
                js.Close();
                js = null;
            }

            charset = null;
            return encoding.GetString(data);
        }

        #endregion

        #region 文件和图片上传  有待改进

        /// <summary>
        /// 上传图片到服务器
        /// </summary>
        /// <param name="url">网页地址URL</param>
        /// <param name="refer">网页refer</param>
        /// <param name="nvc">Post参数字符串 汉子先进行 HttpUtility.UrlEncode 编码</param>
        /// <param name="image">图片</param>
        /// <param name="houzhui">图片后缀</param>
        /// <param name="encoding">返回网页编码 可为空[null]/自动识别编码</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns></returns>
        public static string UplodaImage(string url, string refer, NameValueCollection nvc, Image image, string houzhui, Encoding encoding, ref string cookies)
        {
            if (image == null || string.IsNullOrEmpty(houzhui))
            {
                return string.Empty;
            }
            houzhui = houzhui.ToLower();
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");//post信息分隔符
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;//设置http代理
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Referer = refer;
                request.Method = "POST";
                request.Timeout = 10000;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.KeepAlive = true;
                request.Headers.Add("cookie:" + cookies);//将页面的Cookies传入提交页面
                request.Credentials = CredentialCache.DefaultCredentials;

                StringBuilder sb = new StringBuilder();
                boundary = "--" + boundary;
                if (nvc != null && nvc.Count > 0)
                {
                    foreach (string key in nvc.Keys)
                    {
                        sb.Append(boundary);
                        sb.Append("\r\n");
                        sb.Append("Content-Disposition: form-data; name=\"" + key + "\"\r\n\r\n");
                        sb.Append(nvc[key]);//sb.Append(HttpUtility.UrlEncode(nvc[key]));
                        sb.Append("\r\n");
                    }
                }
                string filename = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now.AddHours(-2));
                sb.Append(boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + filename + "." + houzhui + "\"\r\n");
                //sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"file." + houzhui + "\"\r\n");
                sb.Append("Content-Type: image/" + houzhui + "\r\n\r\n");

                byte[] postdata = Encoding.UTF8.GetBytes(sb.ToString());

                MemoryStream Ms = new MemoryStream();
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Jpeg;
                switch (houzhui)
                {
                    case "gif":
                        format = System.Drawing.Imaging.ImageFormat.Gif;
                        break;
                    case "bmp":
                        format = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;
                    case "png":
                        format = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    default:
                        break;
                }
                image.Save(Ms, format);
                byte[] filedata = new byte[Ms.Length];
                Ms.Position = 0;
                Ms.Read(filedata, 0, Convert.ToInt32(Ms.Length));
                Ms.Close();

                byte[] enddata = Encoding.UTF8.GetBytes("\r\n" + boundary + "--\r\n");
                request.ContentLength = postdata.Length + filedata.Length + enddata.Length;

                Stream stream = request.GetRequestStream();
                stream.Write(postdata, 0, postdata.Length);//把Post数据写入HttpWebRequest的Request流
                stream.Write(filedata, 0, filedata.Length);//把上传文件数据写入HttpWebRequest的Request流
                stream.Write(enddata, 0, enddata.Length);//把结尾符数据写入HttpWebRequest的Request流

                stream.Close();
                stream = null;
                boundary = null;
                sb = null;
                postdata = null;
                filedata = null;
                enddata = null;

                response = (HttpWebResponse)request.GetResponse();
                string cookieout = response.Headers["Set-Cookie"];//获取页面的Cookies
                cookies = CookiesProcess(cookies, cookieout);
                cookieout = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (request != null)
                {
                    request = null;
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 上传图片到服务器
        /// </summary>
        /// <param name="url">网页地址URL</param>
        /// <param name="refer">网页refer</param>
        /// <param name="nvc">Post参数字符串 汉子先进行 HttpUtility.UrlEncode 编码</param>
        /// <param name="filePath">本地文件路径</param>
        /// <param name="encoding">返回网页编码 可为空[null]/自动识别编码</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns></returns>
        public static string UplodaImage(string url, string refer, NameValueCollection nvc, string filePath, Encoding encoding, ref string cookies)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return string.Empty;
            }
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");//post信息分隔符
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;//设置http代理
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Referer = refer;
                request.Method = "POST";
                request.Timeout = 10000;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.KeepAlive = true;
                request.Headers.Add("cookie:" + cookies);//将页面的Cookies传入提交页面
                request.Credentials = CredentialCache.DefaultCredentials;

                StringBuilder sb = new StringBuilder();
                boundary = "--" + boundary;
                if (nvc != null && nvc.Count > 0)
                {
                    foreach (string key in nvc.Keys)
                    {
                        sb.Append(boundary);
                        sb.Append("\r\n");
                        sb.Append("Content-Disposition: form-data; name=\"" + key + "\"\r\n\r\n");
                        sb.Append(nvc[key]);//sb.Append(HttpUtility.UrlEncode(nvc[key]));
                        sb.Append("\r\n");
                    }
                }
                sb.Append(boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + new FileInfo(filePath).Name + "\"\r\n");
                sb.Append("Content-Type: image/" + filePath.Substring(filePath.LastIndexOf(".") + 1) + "\r\n\r\n");

                byte[] postdata = Encoding.UTF8.GetBytes(sb.ToString());
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] filedata = new byte[fileStream.Length];
                fileStream.Read(filedata, 0, filedata.Length);
                byte[] enddata = Encoding.UTF8.GetBytes("\r\n" + boundary + "--\r\n");
                request.ContentLength = postdata.Length + filedata.Length + enddata.Length;

                Stream stream = request.GetRequestStream();
                stream.Write(postdata, 0, postdata.Length);//把Post数据写入HttpWebRequest的Request流
                stream.Write(filedata, 0, filedata.Length);//把上传文件数据写入HttpWebRequest的Request流
                stream.Write(enddata, 0, enddata.Length);//把结尾符数据写入HttpWebRequest的Request流

                stream.Close();
                stream = null;
                boundary = null;
                sb = null;
                postdata = null;
                filedata = null;
                enddata = null;

                response = (HttpWebResponse)request.GetResponse();
                string cookieout = response.Headers["Set-Cookie"];//获取页面的Cookies
                cookies = CookiesProcess(cookies, cookieout);
                cookieout = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (request != null)
                {
                    request = null;
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 上传文本文件到服务器
        /// </summary>
        /// <param name="url">网页地址URL</param>
        /// <param name="refer">网页refer</param>
        /// <param name="nvc">Post参数字符串 汉子先进行 HttpUtility.UrlEncode 编码</param>
        /// <param name="filePath">本地文件路径</param>
        /// <param name="encoding">返回网页编码 可为空[null]/自动识别编码</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns></returns>
        public static string UplodaFiles(string url, string refer, NameValueCollection nvc, string filePath, Encoding encoding, ref string cookies)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return string.Empty;
            }
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");//post信息分隔符
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;//设置http代理
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Referer = refer;
                request.Method = "POST";
                request.Timeout = 10000;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.KeepAlive = true;
                request.Headers.Add("cookie:" + cookies);//将页面的Cookies传入提交页面
                request.Credentials = CredentialCache.DefaultCredentials;

                StringBuilder sb = new StringBuilder();
                boundary = "--" + boundary;
                if (nvc != null && nvc.Count > 0)
                {
                    foreach (string key in nvc.Keys)
                    {
                        sb.Append(boundary);
                        sb.Append("\r\n");
                        sb.Append("Content-Disposition: form-data; name=\"" + key + "\"\r\n\r\n");
                        sb.Append(nvc[key]);//sb.Append(HttpUtility.UrlEncode(nvc[key]));
                        sb.Append("\r\n");
                    }
                }
                sb.Append(boundary);
                sb.Append("\r\n");
                sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + new FileInfo(filePath).Name + "\"\r\n");
                sb.Append("Content-Type: text/plain\r\n\r\n");

                byte[] postdata = Encoding.UTF8.GetBytes(sb.ToString());
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] filedata = new byte[fileStream.Length];
                fileStream.Read(filedata, 0, filedata.Length);
                byte[] enddata = Encoding.UTF8.GetBytes("\r\n" + boundary + "--\r\n");
                request.ContentLength = postdata.Length + filedata.Length + enddata.Length;

                Stream stream = request.GetRequestStream();
                stream.Write(postdata, 0, postdata.Length);//把Post数据写入HttpWebRequest的Request流
                stream.Write(filedata, 0, filedata.Length);//把上传文件数据写入HttpWebRequest的Request流
                stream.Write(enddata, 0, enddata.Length);//把结尾符数据写入HttpWebRequest的Request流

                stream.Close();
                stream = null;
                boundary = null;
                sb = null;
                postdata = null;
                filedata = null;
                enddata = null;

                response = (HttpWebResponse)request.GetResponse();
                string cookieout = response.Headers["Set-Cookie"];//获取页面的Cookies
                cookies = CookiesProcess(cookies, cookieout);
                cookieout = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
                return "发送失败";
            }
            finally
            {
                if (request != null)
                {
                    request = null;
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return string.Empty;
        }

        #endregion

        #region  Public Methods 公有方法

        #region GetPage 获取网页内容

        /// <summary>
        /// 从服务器上下载发送的广告图片
        /// </summary>
        /// <param name="url">图片地址</param>
        /// <param name="cookies">cookies</param>
        /// <returns></returns>
        public static Image GetImage(string url, ref string cookies)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;
                request.Timeout = 5000;//5秒没有反应就退出
                request.AllowAutoRedirect = false;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Add("cookie:" + cookies);//将页面的Cookies传入提交页面
                response = (HttpWebResponse)request.GetResponse();
                string cookieout = response.Headers["Set-Cookie"];//获取页面的Cookies
                cookies = CookiesProcess(cookies, cookieout);
                cookieout = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        Image image = Image.FromStream(stream);
                        return image;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// 获取网页内容 带cookie  指定编码/null为自动编码
        /// </summary>
        /// <param name="url">网页地址URL</param>
        /// <param name="refer">网页refer</param>
        /// <param name="encoding">网页编码 可为空[null]/自动识别编码</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns>返回网页html(失败则为空字符)</returns>
        public static string GetPage(string url, string refer, Encoding encoding, ref string cookies)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;//设置http代理
                request.Referer = refer;
                request.Timeout = 5000;//5秒没有反应就退出
                request.AllowAutoRedirect = false;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.Connection = "keep-alive";
                //request.KeepAlive = true;
                request.Headers.Add("cookie:" + cookies);//将页面的Cookies传入提交页面
                response = (HttpWebResponse)request.GetResponse();
                string cookieout = response.Headers["Set-Cookie"];//获取页面的Cookies
                cookies = CookiesProcess(cookies, cookieout);
                cookieout = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (request != null)
                {
                    request = null;
                }
                if (response != null)
                {
                    response.Close();
                }
                encoding = null;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取网页内容 指定编码/null为自动编码
        /// </summary>
        /// <param name="url">网页地址URL</param>
        /// <param name="encoding">网页编码 可为空[null]/自动识别编码</param>
        /// <returns>返回网页html(失败则为空字符)</returns>
        public static string GetPage(string url, Encoding encoding)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;//设置http代理
                request.Timeout = 5000;//5秒没有反应就退出
                request.AllowAutoRedirect = false;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //request.ContentType = "text/html";//application/x-www-form-urlencoded
                //request.Connection = "keep-alive";
                //request.KeepAlive = true;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (request != null)
                {
                    request = null;
                }
                if (response != null)
                {
                    response.Close();
                }
                encoding = null;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取网页内容 自动识别编码
        /// </summary>
        /// <param name="url">网页地址URL</param>
        /// <returns></returns>
        public static string GetPage(string url)
        {
            return GetPage(url, null);
        }

        /// <summary>
        /// 获取网页内容 带cookie  指定编码/null为自动编码
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="encoding">网页编码 可为空[null]/自动识别编码</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns>返回网页html(失败则为空字符)</returns>
        public static string GetPage(HttpWebRequest request, Encoding encoding, ref string cookies)
        {
            HttpWebResponse response = null;
            try
            {
                request.Proxy = Proxy;//设置http代理
                request.Method = "GET";
                request.Headers.Add("cookie:" + cookies);//将页面的Cookies传入提交页面
                response = (HttpWebResponse)request.GetResponse();
                string cookieout = response.Headers["Set-Cookie"];//获取页面的Cookies
                cookies = CookiesProcess(cookies, cookieout);
                cookieout = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                encoding = null;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取网页内容 带cookie 自动识别编码
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns></returns>
        public static string GetPage(HttpWebRequest request, ref string cookies)
        {
            return GetPage(request, null, ref cookies);
        }

        /// <summary>
        /// 获取网页内容 指定编码/null为自动编码
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="encoding">网页编码 可为空[null]/自动识别编码</param>
        /// <returns>返回网页html(失败则为空字符)</returns>
        public static string GetPage(HttpWebRequest request, Encoding encoding)
        {
            HttpWebResponse response = null;
            try
            {
                request.Proxy = Proxy;//设置http代理
                request.Method = "GET";
                request.Timeout = 5000;//5秒没有反应就退出
                request.AllowAutoRedirect = false;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //request.ContentType = "text/html";//application/x-www-form-urlencoded
                //request.Connection = "keep-alive";
                //request.KeepAlive = true;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                encoding = null;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取网页内容 自动识别编码
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <returns></returns>
        public static string GetPage(HttpWebRequest request)
        {
            return GetPage(request, null);
        }

        #endregion

        #region GetPageByPost Post提交获取网页内容

        /// <summary>
        /// Post页面，得到Post传回来的值 [Post字符串采用UTF8编码]
        /// </summary>
        /// <param name="url">网页地址URL</param>
        /// <param name="refer">网页refer</param>
        /// <param name="postdata">Post参数字符串（简单的例子直接用Firefox得到的post数据做测试）[参数用&连接] Post数据不需要编码 </param>
        /// <param name="encoding">返回网页编码 可为空[null]/自动识别编码</param>
        /// <param name="postencoding">提交Post串的字符编码 可为空[null]/采用默认utf-8编码</param>
        /// <returns></returns>
        public static string GetPageByPost(string url, string refer, string postdata, Encoding encoding, Encoding postencoding)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                byte[] postData = postencoding == null ? Encoding.UTF8.GetBytes(postdata) : postencoding.GetBytes(postdata);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;//设置http代理
                request.Referer = refer;
                request.Method = "POST";
                request.Timeout = 5000;
                request.AllowAutoRedirect = false;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "text/html";//application/x-www-form-urlencoded
                //request.Connection = "keep-alive";
                //request.KeepAlive = true;
                request.ContentLength = postData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postData, 0, postData.Length);//把数据写入HttpWebRequest的Request流
                stream.Close();
                stream = null;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (request != null)
                {
                    request = null;
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Post页面，得到Post传回来的值 [Post字符串采用UTF8编码]
        /// </summary>
        /// <param name="url">网页地址URL</param>
        /// <param name="refer">网页refer</param>
        /// <param name="postdata">Post参数字符串（简单的例子直接用Firefox得到的post数据做测试）[参数用&连接] Post数据不需要编码 </param>
        /// <param name="encoding">返回网页编码 可为空[null]/自动识别编码</param>
        /// <param name="postencoding">提交Post串的字符编码 可为空[null]/采用默认utf-8编码</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns></returns>
        public static string GetPageByPost(string url, string refer, string postdata, Encoding encoding, Encoding postencoding, ref string cookies)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                byte[] postData = postencoding == null ? Encoding.UTF8.GetBytes(postdata) : postencoding.GetBytes(postdata);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = Proxy;//设置http代理
                request.Referer = refer;
                request.Method = "POST";
                request.Timeout = 5000;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.ContentType = "application/x-www-form-urlencoded";//text/html
                //request.Connection = "keep-alive";
                //request.KeepAlive = true;
                request.Headers.Add("cookie:" + cookies);//将页面的Cookies传入提交页面
                request.ContentLength = postData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postData, 0, postData.Length);//把数据写入HttpWebRequest的Request流
                stream.Close();
                stream = null;
                response = (HttpWebResponse)request.GetResponse();
                string cookieout = response.Headers["Set-Cookie"];//获取页面的Cookies
                cookies = CookiesProcess(cookies, cookieout);
                cookieout = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (request != null)
                {
                    request = null;
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Post页面，得到Post传回来的值 [Post字符串采用UTF8编码]
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="postdata">Post参数字符串（简单的例子直接用Firefox得到的post数据做测试）[参数用&连接] Post数据不需要编码 </param>
        /// <param name="encoding">返回网页编码 可为空[null]/自动识别编码</param>
        /// <param name="postencoding">提交Post串的字符编码 可为空[null]/采用默认utf-8编码</param>
        /// <returns></returns>
        public static string GetPageByPost(HttpWebRequest request, string postdata, Encoding encoding, Encoding postencoding)
        {
            HttpWebResponse response = null;
            try
            {
                byte[] postData = postencoding == null ? Encoding.UTF8.GetBytes(postdata) : postencoding.GetBytes(postdata);
                request.Method = "POST";
                request.Proxy = Proxy;//设置http代理
                request.ContentLength = postData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postData, 0, postData.Length);//把数据写入HttpWebRequest的Request流
                stream.Close();
                stream = null;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Post页面，得到Post传回来的值 [Post字符串采用UTF8编码] 自动识别返回内容的编码
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="postdata">Post参数字符串（简单的例子直接用Firefox得到的post数据做测试）[参数用&连接] Post数据不需要编码</param>
        /// <returns></returns>
        public static string GetPageByPost(HttpWebRequest request, string postdata)
        {
            return GetPageByPost(request, postdata, null, null);
        }

        /// <summary>
        /// Post页面，得到Post传回来的值 [Post字符串采用UTF8编码]
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="postdata">Post参数字符串（简单的例子直接用Firefox得到的post数据做测试）[参数用&连接] Post数据不需要编码 </param>
        /// <param name="encoding">返回网页编码 可为空[null]/自动识别编码</param>
        /// <param name="postencoding">提交Post串的字符编码 可为空[null]/采用默认utf-8编码</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns></returns>
        public static string GetPageByPost(HttpWebRequest request, string postdata, Encoding encoding, Encoding postencoding, ref string cookies)
        {
            HttpWebResponse response = null;
            try
            {
                byte[] postData = postencoding == null ? Encoding.UTF8.GetBytes(postdata) : postencoding.GetBytes(postdata);
                request.Method = "POST";
                request.Proxy = Proxy;//设置http代理
                request.Headers.Add("cookie:" + cookies);//将页面的Cookies传入提交页面
                request.ContentLength = postData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postData, 0, postData.Length);//把数据写入HttpWebRequest的Request流
                stream.Close();
                stream = null;
                response = (HttpWebResponse)request.GetResponse();
                string cookieout = response.Headers["Set-Cookie"];//获取页面的Cookies
                cookies = CookiesProcess(cookies, cookieout);
                cookieout = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return StreamToString(response, encoding);
                }
            }
            catch
            {
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Post页面，得到Post传回来的值 [Post字符串采用UTF8编码] 自动识别返回内容的编码
        /// </summary>
        /// <param name="request">HttpWebRequest对象</param>
        /// <param name="postdata">Post参数字符串（简单的例子直接用Firefox得到的post数据做测试）[参数用&连接] Post数据不需要编码</param>
        /// <param name="cookies">传入传出Cookie 用英文;分分隔</param>
        /// <returns></returns>
        public static string GetPageByPost(HttpWebRequest request, string postdata, ref string cookies)
        {
            return GetPageByPost(request, postdata, null, null, ref cookies);
        }

        #endregion

        #endregion
    }
}
