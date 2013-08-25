namespace CodeUnion.Utility.Caching
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Xml.Linq;

    /// <summary>
    /// 缓存工具类
    /// </summary>
    public class CacheUtility
    {
        private static string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/ConfigInfo.xml";

        /// <summary>
        /// 获取配置文件的配置值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValue(string key)
        {
            XElement root = XElement.Load(configFilePath);
            return root.Descendants(key).ToList()[0].Value;
        }

        /// <summary>
        /// 插入Cache 
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        public static void Add<T>(string key, T value)
        {
            HttpContext.Current.Cache.Insert(
                key,
                value,
                null,
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                TimeSpan.FromMinutes(ExpireTimeSpan));
        }

        /// <summary> 
        /// 删除指定的Cache 
        /// </summary> 
        /// <param name="key">缓存键</param> 
        public static void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        /// <summary> 
        /// 判断Cache是否存在 
        /// </summary> 
        /// <param name="key">缓存键</param> 
        /// <returns></returns> 
        public static bool Exists(string key)
        {
            return HttpContext.Current.Cache[key] != null;
        }

        /// <summary> 
        /// 取得Cache值(泛型方法)
        /// </summary> 
        /// <typeparam name="T">缓存对象类型</typeparam> 
        /// <param name="key">缓存键</param> 
        /// <param name="value">缓存值</param> 
        /// <returns></returns> 
        public static bool Get<T>(string key, out T value)
        {
            try
            {
                if (!Exists(key))
                {
                    value = default(T);
                    return false;
                }
                value = (T)HttpContext.Current.Cache[key];
            }
            catch
            {
                value = default(T);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        private static int ExpireTimeSpan
        {
            get
            {
                int iTimeSpan;
                string strExpireTimeSpan = GetConfigValue("ExpireTimeSpan");
                if (strExpireTimeSpan == null)
                {
                    return 30;
                }
                if (!int.TryParse(strExpireTimeSpan, out iTimeSpan))
                {
                    return 30;
                }
                return iTimeSpan;
            }
        }
    }
}
