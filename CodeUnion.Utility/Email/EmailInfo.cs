using System;
using System.Collections.Generic;

namespace CodeUnion.Utility.Email
{
    /// <summary>
    /// 邮件信息配置Model
    /// </summary>
    [Serializable]
    public class EmailInfo
    {
        #region 属性

        /// <summary>
        /// 发送邮件服务器地址
        /// </summary>
        private string emailServer = "smtp.163.com";

        /// <summary>
        /// 发件人地址
        /// </summary>
        private string emailFrom = "guochuande1987@163.com";

        /// <summary>
        /// 收件人地址集合
        /// </summary>
        public List<string> emailTo;

        /// <summary>
        /// 抄送人地址集合
        /// </summary>
        private List<string> emailCC;

        /// <summary>
        /// 邮件主题
        /// </summary>
        private string emailTitle = "SEO-Google Analytics统计数据";

        /// <summary>
        /// 邮件正文
        /// </summary>
        private string emailBody = "SEO-Google Analytics统计数据！";

        /// <summary>
        /// 附件集合（全路径 例：D:\正则表达式大全邮箱和手机的验证.txt）
        /// </summary>
        private List<string> emailAttachments;

        /// <summary>
        /// 发送人用户名
        /// </summary>
        private string userName = "guochuande1987@163.com";

        /// <summary>
        /// 发送人密码
        /// </summary>
        private string passWord = "gcd19871015";

        #endregion

        #region 构造函数

        public EmailInfo()
        {
            //空构造函数
        }

        /// <summary>
        /// 没有抄送和附件
        /// </summary>
        public EmailInfo(string _emailServer, string _emailFrom, List<string> _emailTo, string _emailTitle, string _emailBody, string _userName, string _passWord)
        {
            this.emailServer = _emailServer;
            this.emailFrom = _emailFrom;
            this.emailTo = _emailTo;
            this.emailTitle = _emailTitle;
            this.emailBody = _emailBody;
            this.userName = _userName;
            this.passWord = _passWord;
        }

        /// <summary>
        /// 包含抄送和附件
        /// </summary>
        public EmailInfo(string _emailServer, string _emailFrom, List<string> _emailTo, string _emailTitle, string _emailBody, string _userName, string _passWord, List<string> _emailCC, List<string> _emailAttachments)
        {
            this.emailServer = _emailServer;
            this.emailFrom = _emailFrom;
            this.emailTo = _emailTo;
            this.emailTitle = _emailTitle;
            this.emailBody = _emailBody;
            this.userName = _userName;
            this.passWord = _passWord;
            this.emailCC = _emailCC;
            this.emailAttachments = _emailAttachments;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 发送邮件服务器地址（例：使用163邮箱发邮件，那么就是： smtp.163.com）
        /// </summary>
        public string EmailServer
        {
            get { return emailServer; }
            set { emailServer = value; }
        }

        /// <summary>
        /// 发件人地址（例：egaaaa@163.com）
        /// </summary>
        public string EmailFrom
        {
            get { return emailFrom; }
            set { emailFrom = value; }
        }

        /// <summary>
        /// 收件人地址集合
        /// </summary>
        public List<string> EmailTo
        {
            get { return emailTo; }
            set { emailTo = value; }
        }

        /// <summary>
        /// 抄送人地址集合
        /// </summary>
        public List<string> EmailCC
        {
            get { return emailCC; }
            set { emailCC = value; }
        }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string EmailTitle
        {
            get { return emailTitle; }
            set { emailTitle = value; }
        }

        /// <summary>
        /// 邮件正文
        /// </summary>
        public string EmailBody
        {
            get { return emailBody; }
            set { emailBody = value; }
        }

        /// <summary>
        /// 附件集合（全路径 例：D:\正则表达式大全邮箱和手机的验证.txt）
        /// </summary>
        public List<string> EmailAttachments
        {
            get { return emailAttachments; }
            set { emailAttachments = value; }
        }

        /// <summary>
        /// 发送人用户名
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// 发送人密码
        /// </summary>
        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; }
        }

        #endregion
    }
}
