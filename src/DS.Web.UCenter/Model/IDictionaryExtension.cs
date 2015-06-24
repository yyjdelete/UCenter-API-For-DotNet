using System;
using System.Collections;

namespace DS.Web.UCenter
{
    /// <summary>   
    /// Dozer 版权所有
    /// 允许复制、修改，但请保留我的联系方式！
    /// http://www.dozer.cc
    /// mailto:dozer.cc@gmail.com
    /// </summary>
    static class DictionaryExtension
    {

        /// <summary>
        /// 得到Int
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static public int GetInt(this IDictionary data, string key)
        {
            int result;
            TryGet(data, key, out result);
            return result;
        }


        /// <summary>
        /// 得到String
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static public string GetString(this IDictionary data, string key)
        {
            string result;
            TryGet(data, key, out result, String.Empty);
            return result;
        }


        /// <summary>
        /// 得到Bool
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        static public bool GetBool(this IDictionary data, string key, string compare = "1")
        {
            var result = default(bool);
            object obj;
            if (TryGetRaw(data, key, out obj))
            {
                result = Object.Equals(compare, obj);
            }
            return result;
        }


        /// <summary>
        /// 得到DateTime
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static public DateTime GetDateTime(this IDictionary data, string key)
        {
            var result = default(DateTime);
            long tmp;
            if (TryGet(data, key, out tmp))
            {
                result = UcUtility.PhpTimeToDateTime(long.Parse(data[key].ToString()));
            }
            return result;
        }


        /// <summary>
        /// 得到Double
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static public double GetDouble(this IDictionary data, string key)
        {
            var result = default(double);
            TryGet(data, key, out result);
            return result;
        }


        /// <summary>
        /// 得到Hashtable
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static public Hashtable GetHashtable(this IDictionary data, string key)
        {
            var result = default(Hashtable);
            object obj;
            if (TryGetRaw(data, key, out obj))
            {
                if (obj is Hashtable)
                    result = (Hashtable)obj;
            }
            return result;
        }

        private static bool TryGetRaw(this IDictionary dict, string key, out object result)
        {
            result = default(object);
            if (dict != null && dict.Contains(key))
            {
                result = dict[key];
                return true;
            }
            return false;
        }

        private static bool TryGet<T>(this IDictionary dict, string key, out T result, T defVal=default(T))
        {
            result = defVal;
            object obj = default(object);
            if (TryGetRaw(dict, key, out obj))
            {
                try
                {
                    result = (T)Convert.ChangeType(obj, typeof(T));
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}
