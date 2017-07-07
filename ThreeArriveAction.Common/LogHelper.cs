using System;
using System.IO;
using System.Text;

namespace ThreeArriveAction.Common
{
    public static class LogHelper
    {
        public static string path = Environment.CurrentDirectory;

        public static void Log(string msg, string title = "", LogTypeEnum type = LogTypeEnum.Info)
        {
            path += $"\\log\\{DateTime.Now.ToShortDateString()}.log";
            var str = new StringBuilder();
            str.AppendLine("----------------------------------------------------------");
            str.AppendLine("时间:");
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
