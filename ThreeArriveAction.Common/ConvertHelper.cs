using System;
using Newtonsoft.Json;

namespace ThreeArriveAction.Common
{
    public static class ConvertHelper
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static int ToInt(this object o)
        {
            try
            {
                return Convert.ToInt32(o);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message, o + "int类型转换失败---VALUE:" + o);
                return 0;
            }
        }


        /// <summary>  
        /// 获取当前时间戳  
        /// </summary>  
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>  
        /// <returns></returns>  
        public static string GetTimeStamp(bool bflag = true)
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var ret = bflag ? Convert.ToInt64(ts.TotalSeconds).ToString() : Convert.ToInt64(ts.TotalMilliseconds).ToString();
            return ret;
        }

        /// <summary>
        /// 一定长度的随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetNonce(int length)
        {
            const string n = "qwertyuioplkjhgfdsazxcvbnm1234567890QWERTYUIOPLKJHGFDSAZXCVBNM";
            var s = "";
            var r = new Random();
            for (var i = 0; i < length; i++)
            {
                s += n[r.Next(0, n.Length)];
            }
            return s;
        }
    }
}
