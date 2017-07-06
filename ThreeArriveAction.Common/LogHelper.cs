namespace ThreeArriveAction.Common
{
    public class LogHelper
    {
        public static void Log(string msg, string title = "", LogTypeEnum type = LogTypeEnum.Info)
        {

        }
    }

    public enum LogTypeEnum
    {
        Info,
        Waiting,
        Error
    }
}
