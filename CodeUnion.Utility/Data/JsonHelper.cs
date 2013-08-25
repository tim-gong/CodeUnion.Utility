namespace CodeUnion.Utility.Data
{
    using System.Web.Script.Serialization;

    public static class JsonHelper
    {
        /// <summary>
        /// 将实体序列化为Json对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }
    }
}
