using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeUnion.Utility.Data
{
    public static class ExcelHelper
    {
        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="list">数据列表</param>
        /// <param name="tmppath">模板文件（空的excel文件）地址</param>
        /// <param name="outputpath">要输出的地址</param>
        /// <param name="sheetname">excel的sheet名称</param>
        /// <param name="propdict">要导出的列名列表</param>
        /// <returns></returns>
        public static bool ExportToExcel<T>(IList<T> list, string tmppath, string outputpath, string sheetname, NameValueCollection propdict)
            where T : class
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentNullException("list", "该列表没有实际的值");
            }

            #region 判断模板文件、保存路径

            if (!File.Exists(tmppath))
            {
                throw new FileNotFoundException("模板文件路径不正确，找不到模板文件");
            }
            if (File.Exists(outputpath))
            {
                File.Delete(outputpath);
            }
            File.Copy(tmppath, outputpath);

            #endregion

            sheetname = string.IsNullOrEmpty(sheetname) ? "Sheet1" : sheetname;
            string excelconntext = "Provider=Microsoft.Jet.OleDB.4.0;Data Source=" + outputpath + ";Extended Properties = \"Excel 8.0;HDR=Yes;\"";

            using (OleDbConnection cn = new OleDbConnection(excelconntext))
            {
                cn.Open();
                OleDbCommand com = cn.CreateCommand();
                PropertyInfo[] ps = typeof(T).GetProperties();
                List<PropertyInfo> proplist = new List<PropertyInfo>();
                StringBuilder sb = new StringBuilder();
                sb.Append("Create Table [" + sheetname + "$](");

                StringBuilder sb2 = new StringBuilder();
                sb2.Append("Insert into [" + sheetname + "$] values(");
                foreach (var item in ps)
                {
                    string head = propdict[item.Name];
                    if (string.IsNullOrEmpty(head)) continue;
                    proplist.Add(item);
                    sb.Append(head + " ");
                    switch (item.PropertyType.Name)
                    {
                        case "String": sb.Append("Char,"); break;
                        case "Decimal": sb.Append("Decimal,"); break;
                        case "DateTime": sb.Append("Char,"); break;
                        case "Int32": sb.Append("Integer,"); break;
                        default: sb.Append("Char,");
                            break;
                    }
                    sb2.Append("@" + item.Name + " ,");
                }
                com.CommandText = sb.ToString().Trim(',') + ")";
                com.ExecuteNonQuery();//创建表头

                com.CommandText = sb2.ToString().Trim(',') + ")";

                foreach (var item in list)
                {
                    OleDbParameter[] pars = new OleDbParameter[proplist.Count];
                    for (int i = 0; i < proplist.Count; i++)
                    {
                        pars[i] = new OleDbParameter(proplist[i].Name, proplist[i].GetValue(item, null) ?? "");
                    }
                    com.Parameters.Clear();
                    com.Parameters.AddRange(pars);
                    com.ExecuteNonQuery();//写入每行的数据
                }
            }
            return true;
        }
        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <param name="list">数据列表</param>
        /// <param name="tmppath">模板文件（空的excel文件）地址</param>
        /// <param name="outputpath">要输出的地址</param>
        /// <param name="sheetname">excel的sheet名称</param>
        /// <param name="dbParse">要导出的列名列表</param>
        /// <returns></returns>
        public static bool ExportToExcel<T>(IList<T> list, string tmppath, string outputpath, string sheetname, List<DbcolParse> dbParse)
            where T : class
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentNullException("list", "该列表没有实际的值");
            }

            #region 判断模板文件、保存路径

            if (!File.Exists(tmppath))
            {
                throw new FileNotFoundException("模板文件路径不正确，找不到模板文件");
            }
            if (File.Exists(outputpath))
            {
                File.Delete(outputpath);
            }
            File.Copy(tmppath, outputpath);

            #endregion

            sheetname = string.IsNullOrEmpty(sheetname) ? "Sheet1" : sheetname;
            string excelconntext = "Provider=Microsoft.Jet.OleDB.4.0;Data Source=" + outputpath + ";Extended Properties = \"Excel 8.0;HDR=Yes;\"";

            using (OleDbConnection cn = new OleDbConnection(excelconntext))
            {
                PropertyInfo[] ps = typeof(T).GetProperties();
                cn.Open();
                OleDbCommand com = cn.CreateCommand();
                StringBuilder sb = new StringBuilder();
                sb.Append("Create Table [" + sheetname + "$](");
                StringBuilder sb2 = new StringBuilder();
                sb2.Append("Insert into [" + sheetname + "$] values(");
                foreach (var item in dbParse)
                {
                    var propinfo = ps.SingleOrDefault(c => c.Name == item.ColumnName);
                    if (propinfo == null)
                    {
                        dbParse.Remove(item);
                        continue;
                    }
                    item.PropInfo = propinfo;

                    sb.Append(item.Title + " ");

                    if (item.DataParse != null)
                        sb.Append("Char,");
                    else
                        switch (propinfo.PropertyType.Name)
                        {
                            case "String": sb.Append("Char,"); break;
                            case "Decimal": sb.Append("Decimal,"); break;
                            case "DateTime": sb.Append("Char,"); break;
                            case "Int32": sb.Append("Integer,"); break;
                            default: sb.Append("Char,");
                                break;
                        }
                    sb2.Append("@" + item.ColumnName + " ,");
                }
                com.CommandText = sb.ToString().Trim(',') + ")";
                com.ExecuteNonQuery();//创建表头
                com.CommandText = sb2.ToString().Trim(',') + ")";//建立SQL语句
                foreach (var item in list)
                {
                    OleDbParameter[] pars = new OleDbParameter[dbParse.Count];
                    for (int i = 0; i < dbParse.Count; i++)
                    {
                        DbcolParse dp = dbParse[i];
                        object orgval = dp.PropInfo.GetValue(item, null);
                        object val = dp.DataParse != null ? (orgval != null ? dp.DataParse(orgval) : string.Empty) : orgval ?? string.Empty;
                        pars[i] = new OleDbParameter(dbParse[i].ColumnName, val);
                    }
                    com.Parameters.Clear();
                    com.Parameters.AddRange(pars);
                    com.ExecuteNonQuery();//写入每行的数据
                }
            }
            return true;
        }
    }
    /// <summary>
    /// 数据列解析类
    /// </summary>
    public class DbcolParse
    {
        /// <summary>
        /// 源数据列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Excel列头
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 数据解析
        /// </summary>
        public Func<object, string> DataParse { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        internal PropertyInfo PropInfo { get; set; }
    }
}
