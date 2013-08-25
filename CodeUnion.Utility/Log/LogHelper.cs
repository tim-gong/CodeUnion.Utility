using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeUnion.Utility.Log
{
    public static class LogHelper
    {
        #region 写文本

        /// <summary>
        /// 将一个字符串写入指定的文本[覆盖原有数据]（如果文本|路径不存在则创建文本|路径）
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <param name="row">字符串</param>
        /// <returns>成功返回true</returns>
        public static bool TextWriteOverride(string path, string row)
        {
            try
            {
                string path1 = path.Substring(0, path.LastIndexOf("\\"));
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(path1);
                }
                path1 = null;

                FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                streamWriter.WriteLine(row);
                streamWriter.Close();
                fileStream.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将一个字符串写入指定的文本[在文本后面追加行]（如果文本|路径存在则创建文本|路径）
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <param name="row">字符串</param>
        /// <returns>成功返回true</returns>
        public static bool TextWrite(string path, string row)
        {
            try
            {
                string path1 = path.Substring(0, path.LastIndexOf("\\"));
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(path1);
                }
                path1 = null;

                StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);

                sw.WriteLine(row);

                sw.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将一个字符串数组写入指定的文本[在文本后面追加行]（如果文本|路径存在则创建文本|路径）
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <param name="strs">字符串数组</param>
        /// <returns>成功返回true</returns>
        public static bool TextWrite(string path, List<string> strs)
        {
            if (strs != null && strs.Count > 0)
            {
                try
                {
                    string path1 = path.Substring(0, path.LastIndexOf("\\"));
                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(path1);
                    }
                    path1 = null;

                    StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);

                    foreach (string row in strs)
                    {
                        sw.WriteLine(row);
                    }

                    sw.Close();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 将一个字符串数组写入指定的文本[覆盖原有数据]（如果文本|路径存在则创建文本|路径）
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <param name="strs">字符串数组</param>
        /// <returns>成功返回true</returns>
        public static bool TextWriteOverride(string path, List<string> strs)
        {
            if (strs != null && strs.Count > 0)
            {
                try
                {
                    string path1 = path.Substring(0, path.LastIndexOf("\\"));
                    if (!Directory.Exists(path1))
                    {
                        Directory.CreateDirectory(path1);
                    }
                    path1 = null;
                    FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                    StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                    foreach (string row in strs)
                    {
                        streamWriter.WriteLine(row);
                    }
                    streamWriter.Close();
                    fileStream.Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        #endregion

        #region 读文本

        /// <summary>
        /// 读文本，返回文本行集合
        /// </summary>
        /// <param name="path">文本所在路径</param>
        /// <returns></returns>
        public static List<string> TextReadRows(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            try
            {
                List<string> strs = new List<string>();
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string str;
                while ((str = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(str.Trim()))
                    {
                        strs.Add(str);
                    }
                }
                return strs;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 读文本，返回文本内容
        /// </summary>
        /// <param name="path">文本所在路径</param>
        /// <returns></returns>
        public static string TextReadRow(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }
            try
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                return sr.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        /// <summary>
        /// 删除指定路径的文件
        /// </summary>
        /// <param name="path">要删除文件路径</param>
        /// <returns>成功返回真，否则返回假</returns>
        public static bool DeleteLog(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                File.Delete(path);//创建写入文件

                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取文件夹下所有指定类型的文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="strExPrentFile">文件后缀名</param>
        /// <returns>返回所有文件的完整路径</returns>
        public static List<string> GetFilePath(string path, string strExPrentFile)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);

                FileInfo[] fileInfos = dirInfo.GetFiles("*." + strExPrentFile);

                List<string> strs = new List<string>();

                for (int i = 0; i < fileInfos.Length; i++)
                {
                    strs.Add(fileInfos[i].FullName);
                }
                return strs;
            }
            return null;
        }

        /// <summary>
        /// 获取指定路径的文件夹下的所有指定类型的文件（包括该文件夹的子文件）
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="strExPrentFile">文件后缀名</param>
        /// <returns></returns>
        public static List<string> GetAllFiles(string path, string strExPrentFile)
        {
            List<string> filepaths = new List<string>();

            DirectoryInfo di = new DirectoryInfo(path);

            //string[] rootDirs = Directory.GetDirectories(path);//当前目录的子目录：  

            //string[] rootFiles = Directory.GetFiles(path);//当前目录下的文件：  

            FileSystemInfo[] fsi = di.GetFileSystemInfos();

            foreach (FileSystemInfo tempFsi in fsi)
            {
                if (tempFsi.GetType() == typeof(DirectoryInfo))
                {
                    filepaths.AddRange(GetFilePath(tempFsi.FullName, strExPrentFile));
                }
                else
                {
                    filepaths.Add(tempFsi.FullName);
                }
            }
            return filepaths;
        }


        /// <summary>
        /// 删除指定文件夹下的指定文件夹
        /// </summary>
        /// <param name="path">文件夹所在路径</param>
        /// <param name="folderName">文件夹名称</param>
        /// <returns>成功返回真，否则返回假</returns>
        public static bool DeleteFolder(string path, string folderName)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);

                FileSystemInfo[] fsi = di.GetFileSystemInfos();

                foreach (FileSystemInfo tempFsi in fsi)
                {
                    if (tempFsi.Name.Equals(folderName) && tempFsi.GetType() == typeof(DirectoryInfo))
                    {
                        Directory.Delete(tempFsi.FullName, true);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除指定路径下的文件及文件夹
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <returns>成功返回真，否则返回假</returns>
        public static bool DeleteFolder(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);

                FileSystemInfo[] fsi = di.GetFileSystemInfos();

                foreach (FileSystemInfo tempFsi in fsi)
                {
                    if (tempFsi.GetType() == typeof(DirectoryInfo))
                    {   // 删除文件夹
                        Directory.Delete(tempFsi.FullName, true);
                    }
                    else if (tempFsi.GetType() == typeof(FileInfo))
                    {    // 删除文件
                        tempFsi.Delete();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将多个文本文件合并
        /// </summary>
        /// <param name="paths">多个文本文件路径集合</param>
        /// <param name="path">新文本文件路径</param>
        /// <returns>成功返回真</returns>
        public static bool ConsolidatedText(List<string> paths, string path)
        {
            if (paths != null && paths.Count > 0 && path != "")
            {
                try
                {
                    foreach (string list in paths)
                    {
                        List<string> rows = TextReadRows(list);

                        TextWrite(path, rows);
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(exception + ":" + exception.Message, exception);
                }
            }
            return true;
        }

        /// <summary>
        /// 获取多个文本文件的内容
        /// </summary>
        /// <param name="paths">多个文本文件路径集合</param>
        /// <returns>返回内容集合</returns>
        public static List<string> ReaderTxt(List<string> paths)
        {
            List<string> rowss = new List<string>();
            if (paths != null && paths.Count > 0)
            {
                try
                {
                    foreach (string list in paths)
                    {
                        List<string> rows = TextReadRows(list);
                        if (rows != null)
                        {
                            rowss.AddRange(rows);
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(exception + ":" + exception.Message, exception);
                }
            }
            return rowss;
        }

    }
}
