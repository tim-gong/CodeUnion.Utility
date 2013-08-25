using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeUnion.Utility.Converter
{
    /// <summary>
    /// String类型的扩展方法，将字符串转换为指定的类型
    /// </summary>
    public static class StringParser
    {

        public static T Parse<T>(this string value)
        {
            T result = default(T);
            if (!string.IsNullOrEmpty(value))
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
                result = (T)tc.ConvertFrom(value);
            }
            return result;
        }
    }
}
