using System;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace CodeUnion.Utility.Email
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    [Serializable]
    public class EmailSend
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailInfo">emailInfo对象</param>
        /// <returns>发送成功返回真，否则返回假</returns>
        public static bool SendEmail(EmailInfo emailInfo)
        {
            try
            {
                MailMessage myMail = new MailMessage();
                //邮件优先级（高）
                myMail.Priority = MailPriority.High;
                //邮件标题（邮箱主题）
                myMail.Subject = emailInfo.EmailTitle;
                //邮件正文
                myMail.Body = emailInfo.EmailBody;
                //发件人邮箱
                myMail.From = new MailAddress(emailInfo.EmailFrom);
                //给谁发(收件人地址)
                foreach (string str in emailInfo.EmailTo)
                {
                    myMail.To.Add(str);
                }
                //抄送人地址
                if (emailInfo.EmailCC != null)
                {
                    foreach (string str in emailInfo.EmailCC)
                    {
                        myMail.CC.Add(str);
                    }
                }
                //邮件附件
                if (emailInfo.EmailAttachments != null)
                {
                    foreach (string str in emailInfo.EmailAttachments)
                    {
                        myMail.Attachments.Add(new Attachment(str));
                    }
                }
                //发送服务器地址
                SmtpClient smtp = new SmtpClient(emailInfo.EmailServer);
                //发件人登陆用户名和密码
                smtp.Credentials = new NetworkCredential(emailInfo.UserName, emailInfo.PassWord);
                //设置邮件正文为html格式
                myMail.IsBodyHtml = true;
                //设置邮件主题编码
                myMail.SubjectEncoding = Encoding.UTF8;
                //设置邮件正文编码
                myMail.BodyEncoding = Encoding.UTF8;//GetEncoding("gb2312");
                //是否加密连接
                smtp.EnableSsl = false;
                //发送邮件
                smtp.Send(myMail);

                return true;
            }
            catch
            {
                return false;
            }
        }

        //public bool SendEmail()
        //{
        //    try
        //    {
        //        MailMessage myMail = new MailMessage();
        //        myMail.Priority = MailPriority.High;//邮件优先级（高）
        //        myMail.Subject = "Test";//邮件标题（邮箱主题）
        //        myMail.CC.Add("geaaaa@163.com");//抄送人地址
        //        myMail.Attachments.Add(new Attachment(@"D:\正则表达式大全邮箱和手机的验证.txt"));//邮件附件
        //        /* 易@163.com邮箱： pop.163.com; smtp.163.com 
        //         * 网易@yeah.net邮箱： pop.yeah.net; smtp.yeah.net 
        //         * 网易@netease.com邮箱：pop.netease.com; smtp.netease.com
        //         * 网易@126.com邮箱： POP3.126.COM SMTP.126.COM 
        //         */
        //        SmtpClient smtp = new SmtpClient("smtp.163.com");//让其使用指定的 SMTP 服务器和端口发送电子邮件。
        //        myMail.From = new MailAddress("guochuande1987@163.com");
        //        myMail.To.Add("egaaaa@163.com");//给谁发
        //        smtp.Credentials = new NetworkCredential("guochuande1987@163.com", "gcd19871015");//("自己的邮箱", "密码");
        //        myMail.Body = "你好，这个是测试发送";//邮件正文
        //        myMail.IsBodyHtml = true;
        //        myMail.SubjectEncoding = Encoding.GetEncoding("gb2312");
        //        smtp.EnableSsl = false;
        //        smtp.Send(myMail);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

    }
}
