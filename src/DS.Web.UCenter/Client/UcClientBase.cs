using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DS.Web.UCenter.Client
{
    /// <summary>
    /// UCenter API
    /// Dozer 版权所有
    /// 允许复制、修改，但请保留我的联系方式！
    /// http://www.dozer.cc
    /// mailto:dozer.cc@gmail.com
    /// </summary>
    public abstract class UcClientBase
    {
        /// <summary>
        /// 得到加密后的input参数
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected string GetInput(IDictionary<string, string> args)
        {
            args["agent"] = UcUtility.Md5(GetUserAgent());
            args["time"] = UcUtility.PhpTimeNow().ToString();
            return UcUtility.PhpUrlEncode((UcUtility.AuthCodeEncode(ArgsToString(args))));
        }

        /// <summary>
        /// 发送参数
        /// </summary>
        /// <param name="args">参数</param>
        /// <param name="model">Model</param>
        /// <param name="action">Action</param>
        /// <returns></returns>
        protected string SendArgs(IDictionary<string, string> args, string model, string action)
        {
            return SendPost(GetArgsString(args, model, action));
        }

        /// <summary>
        /// 根据参数生成待发送的字符串
        /// </summary>
        /// <param name="args">参数</param>
        /// <param name="model">Model</param>
        /// <param name="action">Action</param>
        /// <returns></returns>
        protected string GetArgsString(IDictionary<string, string> args, string model, string action)
        {
            var input = GetInput(args);
            var api = new Dictionary<string, string>
                          {
                              {"input",input },
                              {"m", model},
                              {"a", action},
                              {"release", UcConfig.UcClientRelease},
                              {"appid", UcConfig.UcAppid}
                          };

            return ArgsToString(api);
        }

        /// <summary>
        /// 对象转换成字符串
        /// </summary>
        /// <param name="args">Dictionary对象</param>
        /// <returns></returns>
        protected string ArgsToString(IEnumerable<KeyValuePair<string, string>> args)
        {
            var sb = new StringBuilder(192);
            foreach (var item in args)
            {
                if (sb.Length != 0) sb.Append('&');
                sb.Append(item.Key).Append('=').Append(item.Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 发送表单并得到返回的字符串数据
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected string SendPost(string args)
        {
            var request = getPostRequest(args);
            return getStr(request).Trim();
        }

        /// <summary>
        /// 发送Get
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        protected string SendGet(string url,IEnumerable<KeyValuePair<string, string>> args)
        {
            var request = getGetRequest(new Uri(url + "?" + ArgsToString(args)));
            return getStr(request).Trim();
        }

        /// <summary>
        /// 处理Response对象，并得到字符串
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string getStr(WebRequest request)
        {
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response == null || response.StatusCode != HttpStatusCode.OK) return "";
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream == null) return "";
                        using (var reader = new StreamReader(stream, UcConfig.UcEncoding))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 得到Requset对象
        /// </summary>
        /// <param name="uri">地址</param>
        /// <returns></returns>
        private HttpWebRequest getGetRequest(Uri uri)
        {
            var request = CreateHttpWebRequest(uri);
            request.Method = "GET";
            return request;
        }

        /// <summary>
        /// 创建基本的Requset对象
        /// </summary>
        /// <param name="uri">地址</param>
        /// <returns></returns>
        private HttpWebRequest CreateHttpWebRequest(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers[HttpRequestHeader.UserAgent] = GetUserAgent();
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "zh-cn");
            request.ServicePoint.Expect100Continue = false;
            return request;
        }

        /// <summary>
        /// 得到Requset对象
        /// </summary>
        /// <param name="data">POST数据</param>
        /// <returns></returns>
        private HttpWebRequest getPostRequest(string data)
        {
            var request = CreateHttpWebRequest(GetUrl());
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = data.Length;

            var encoding = UcConfig.UcEncoding;
            using (var newStream = request.GetRequestStream())
            {
                var dataChars = data.ToCharArray();
                const int len = 128;
                byte[] bytes = new byte[encoding.GetMaxByteCount(len)];
                int strLen;
                for (int i = dataChars.Length, j = 0; j > 0; i -= strLen, j += strLen)
                {
                    strLen = Math.Min(len, i);
                    int bytesLen = encoding.GetBytes(dataChars, i, strLen, bytes, 0);
                    newStream.Write(bytes, 0, bytesLen);
                }
            }

            return request;
        }

        private static Uri baseUri;
        /// <summary>
        /// 得到 Url
        /// </summary>
        /// <returns></returns>
        protected virtual Uri GetUrl()
        {
            return baseUri ?? (baseUri = new Uri(UcConfig.UcApi + "index.php"));
        }

        /// <summary>
        /// 得到 UserAgent 字符串
        /// </summary>
        /// <returns></returns>
        protected virtual string GetUserAgent()
        {
            return UcUtility.GetUserAgent();
        }
    }
}
