using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeUnion.Utility.Data
{
    using System.IO;
    using System.Xml.Serialization;

    public class XmlSerializerHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string SerializeToXml<T>(T t) where T : class, new()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                XmlSerializer ser = new XmlSerializer(typeof(T));
                ser.Serialize(sw, t);
                sw.Dispose();
                return sb.ToString();
            }
            catch
            {
                return "序列化 『" + t.GetType() + "     " + t + " 』失败  ";
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string s) where T : class, new()
        {
            try
            {
                StringReader reader = new StringReader(s);
                XmlSerializer ser = new XmlSerializer(typeof(T));
                T t = ser.Deserialize(reader) as T;
                return t;
            }
            catch
            {
                return default(T);
            }
        }
    }
}
