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
                LogHelper.Log(ex.Message, o.ToString() + "int类型转换失败");
                return 0;
            }
        }
    }
}
