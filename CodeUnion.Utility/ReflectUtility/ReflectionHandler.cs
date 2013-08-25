namespace CodeUnion.Utility.ReflectUtility
{
    using System;
    using System.Reflection;

    using CodeUnion.Utility.Caching;

    public class ReflectionHandler
    {
        private string className = "";
        private Type clstype;
        public ReflectionHandler()
        {
        }
        public ReflectionHandler(string className)
        {
            this.className = className;
        }

        /// <summary>
        /// 实例对象时需要指定类名
        /// </summary>
        /// <param name="assembly">程序集名称</param>
        /// <param name="nameSpace">命名空间</param>
        /// <returns></returns>
        public object GetClassInstance(string assembly, string nameSpace)
        {
            //assembly为程序集名称，nameSpace为命名空间
            this.clstype = Assembly.Load(assembly).GetType(string.Concat(nameSpace, ".", this.className));
            if (this.clstype == null)
                return null;
            object obj = Activator.CreateInstance(this.clstype);
            return obj;
        }

        /// <summary>
        /// 实例对象时不用指定类名  
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="nameSpace"></param>
        /// <param name="classname"></param>
        /// <returns></returns>
        public object GetClassInstance(string assembly, string nameSpace, string classname)
        {
            this.className = classname;
            this.clstype = Assembly.Load(assembly).GetType(string.Concat(nameSpace, ".", classname));
            if (this.clstype == null)
                return null;
            object obj = Activator.CreateInstance(this.clstype);
            return obj;
        }

        /// <summary>
        /// 执行类的静态方法
        /// </summary>
        /// <param name="methodname">类的方法名</param>
        /// <param name="methodtype">方法的参数类型</param>
        /// <param name="parameters">方法的参数</param>
        /// <returns></returns>
        public object GetMethod(string methodname, Type[] methodtype, object[] parameters)
        {
            // methodtype.SetValue(typeof(string),1);  
            System.Reflection.MethodInfo pMethod = this.clstype.GetMethod(methodname, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public, null, methodtype, null);
            //调用方法的一些标志位，这里的含义是Public并且是实例方法，这也是默认的值  
            //System.Reflection.BindingFlags flag = BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public;  
            object returnValue = pMethod.Invoke(null, parameters);
            //string returnValue = pMethod.Invoke(clsObj, flag, Type.DefaultBinder, parameters,null).ToString();  
            return returnValue;
        }

        /// <summary>
        /// 根据类型和方法名获取查询结果集
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        /// <returns></returns>
        public static object GetServiceQueryData<T>(string methodName, object[] parameters)
        {
            Type t = typeof(T);
            object returnValue = null;
            try
            {
                object dObj = Activator.CreateInstance(t);

                //获取方法的信息
                MethodInfo method = t.GetMethod(methodName);
                //调用方法的一些标志位，这里的含义是Public并且是实例方法，这也是默认的值
                BindingFlags flag = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
                //GetValue方法的参数
                //object returnValue = method.Invoke(dObj, flag, Type.DefaultBinder, parameters, null);
                //取得方法返回的值
                returnValue = method.Invoke(dObj, flag, Type.DefaultBinder, parameters, null);
            }
            catch (Exception)
            {
            }
            return returnValue;
        }

        /// <summary>
        /// 根据类型和方法名获取查询结果集
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        public static object GetServiceQueryDataWithCache<T>(string methodName, object[] parameters, string key)
        {
            if (CacheUtility.Exists(key))
            {
                object returnValue;
                if (CacheUtility.Get(key, out returnValue))
                {
                    return returnValue;
                }
                return null;
            }
            return GetServiceQueryData<T>(methodName, parameters);
        }

        /*
        /// <summary>
        /// 根据类型和方法名获取查询结果集
        /// </summary>
        /// <typeparam name="T">类类型</typeparam>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">方法参数</param>
        /// <returns></returns>
        public static object GetServiceQueryDataWithCache<T>(string methodName, object[] parameters) where T : BaseService
        {
            Type t = typeof(T);
            object dObj = Activator.CreateInstance(t);
            MethodInfo method = t.GetMethod("GetDynamicRunSql");
            //调用方法的一些标志位，这里的含义是Public并且是实例方法，这也是默认的值
            BindingFlags flag = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;
            object[] runSqlParameter = new object[2] { methodName, parameters[0] }; // 获取Service查询语句的参数
            string runSql = method.Invoke(dObj, flag, Type.DefaultBinder, runSqlParameter, null) as string;
            if (string.IsNullOrEmpty(runSql))
            {
                return null;
            }
            string key = EncryptUtility.SHA512(runSql);     // 根据动态SQL语句生成128字节的缓存Key
            if (CacheUtility.Exists(key))
            {
                object returnValue;
                if (CacheUtility.Get(key, out returnValue))
                {
                    return returnValue;
                }
                return null;
            }
            return GetServiceQueryData<T>(methodName, parameters);
        }
         */
    }
}
