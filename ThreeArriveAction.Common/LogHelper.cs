using System;
using System.IO;
using System.Text;

namespace ThreeArriveAction.Common
{
    public static class LogHelper
    {
        public static void Log(string msg, string title = "", LogTypeEnum type = LogTypeEnum.Info)
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\log\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            var str = new StringBuilder();
            str.AppendLine("----------------------------------------------------------");
            str.AppendLine("时间:" + DateTime.Now);
            if (!title.IsNullOrEmpty())
            {
                str.AppendLine("标题:" + title);
            }
            str.AppendLine("内容:" + msg);
            str.AppendLine("类型:" + type.ToString());
            str.AppendLine("----------------------------------------------------------\r\n\r\n");
            File.AppendAllText(path, str.ToString(), Encoding.UTF8);
        }
    }

    public enum LogTypeEnum
    {
        Info,
        Waiting,
        Error
    }
}
