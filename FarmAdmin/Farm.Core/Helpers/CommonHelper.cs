using Farm.Infrastructure.CacheStorage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Farm.Infrastructure.Helpers
{
    public class CommonHelper
    {
        /// <summary>
        /// 判断输入的字符串是否是一个合法的手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string input)
        {
            Regex regex = new Regex("^1[34578]\\d{9}$");
            return regex.IsMatch(input);
        }
        public static HttpResponseMessage ToJson<T>(T obj)
        {
            String str;
            if (obj is String || obj is Char)
            {
                str = obj.ToString();
            }
            else
            {
                str = JsonHelper.ToJson<T>(obj);
            }
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
        /// <summary>
        /// 设置Sesseion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout">以分钟为单位</param>
        public static void SetCache<T>(string key, T value, int timeout = 0)
        {
            try
            {
                RedisCache redis = new RedisCache();
                if (timeout > 0)
                {
                    redis.Add(key, value, timeout * 60);
                }
                else
                {
                    redis.Add(key, value);
                }
            }
            catch
            {

            }
        }
        public static T GetCache<T>(string key)
            where T : class
        {
            try
            {

                RedisCache redis = new RedisCache();
                return redis.Get<T>(key);
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<String, Object> ToMap(Object o)
        {
            Dictionary<String, Object> map = new Dictionary<string, object>();

            Type t = o.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(o, new Object[] { }));
                }
            }
        
            return map;

        }


        public static string filterEmoji(String source)
        {
            if (string.IsNullOrEmpty(source)) return "";
            string result = Regex.Replace(source, @"\p{Cs}", "");
            return result;
        }


        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="d">double 型数字</param>
        /// <returns>DateTime</returns>
        public static System.DateTime ConvertIntDateTime(double d)
        {
            try
            {
                System.DateTime time = System.DateTime.MinValue;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                time = startTime.AddMilliseconds(d);
                return time;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>long</returns>
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            try
            {
                //double intResult = 0;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                //intResult = (time- startTime).TotalMilliseconds;
                long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位
                return t;
            }
            catch
            {
                return 0;
            }
        }

        public static string GetConfig(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch
            {
                return "";
            }
        }
    }
}
