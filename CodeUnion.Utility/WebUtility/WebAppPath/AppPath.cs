namespace CodeUnion.Utility.WebUtility.WebAppPath
{
    using System.Configuration;

    public class AppPath
    {
        /// <summary>
        /// 获取主机域名
        /// </summary>
        public static string WebHost
        {
            get
            {
                string host = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
                while (host.LastIndexOf("/") > 6)
                {
                    host = host.Substring(0, host.LastIndexOf("/"));
                }
                return host;
            }
        }

        /// <summary>
        /// 获取应用程序所在的虚拟目录
        /// </summary>
        public static string GetVirtualPath
        {
            get
            {
                string strWebHost = ConfigurationManager.AppSettings["WebHost"];
                if (!string.IsNullOrEmpty(strWebHost))
                {
                    if (strWebHost.EndsWith("/"))
                    {
                        strWebHost = strWebHost.Substring(strWebHost.Length - 1);
                    }
                }
                else
                {
                    strWebHost = WebHost;
                }
                string virtualpath = ConfigurationManager.AppSettings["VirtualPath"];
                if (!string.IsNullOrEmpty(virtualpath))
                {
                    strWebHost = WebHost + "/" + virtualpath;
                }
                return strWebHost;
            }
        }
    }
}
