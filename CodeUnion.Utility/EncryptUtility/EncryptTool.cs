namespace CodeUnion.Utility.EncryptUtility
{
    using System;
    using System.Text;
    using System.Security.Cryptography;

    /// <summary>
    /// 加密操作类
    /// </summary>
    public class EncryptTool
    {
        /// <summary>
        /// 获取字符串的512位哈希值
        /// </summary>
        /// <param name="strPlain"></param>
        /// <returns></returns>
        public static string SHA512(string strPlain)
        {
            SHA512Managed sha512 = new SHA512Managed();
            string strHash = string.Empty;
            byte[] btHash = sha512.ComputeHash(Encoding.Unicode.GetBytes(strPlain));
            for (int i = 0; i < btHash.Length; i++)
            {
                strHash = strHash + Convert.ToString(btHash[i], 16);
            }
            return strHash;
        }
    }
}
