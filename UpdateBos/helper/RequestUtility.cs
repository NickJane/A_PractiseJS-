using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace UpdateBos.helper
{
    public static class RequestUtility
    {
        /// <summary>
        /// 使用Get方法获取字符串结果（暂时没有加入Cookie）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url, Encoding encoding = null)
        {
            WebClient wc = new WebClient();
            wc.Encoding = encoding ?? Encoding.UTF8;
            //if (encoding != null)
            //{
            //    wc.Encoding = encoding;
            //}
            return wc.DownloadString(url);
        }

        /// <summary>
        /// 使用Get方法获取字符串结果（暂时没有加入Cookie）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url, IDictionary<string, string> parameters = null, Encoding encoding = null)
        {
            Encoding readCoding = encoding ?? Encoding.UTF8;

            Uri requestUri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.UserAgent = "mozilla/4.0 (compatible; msie 6.0; windows 2000)";
            request.Method = "Get";
            request.ContentType = "application/x-www-form-urlencoded";

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var item in parameters)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            HttpWebResponse response = null;
            try
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    //若是远程服务器抛出了异常，则捕获并解析
                    response = (HttpWebResponse)ex.Response;
                }

                using (StreamReader sr = new StreamReader(response.GetResponseStream(), readCoding))
                {
                    string content = sr.ReadToEnd();
                    return content;
                }
            }
            finally
            {
                //释放请求的资源
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        ///// <summary>
        ///// Post数据
        ///// </summary>
        ///// <typeparam name="T">模型</typeparam>
        ///// <param name="requestUri">server uri</param>
        ///// <param name="model">数据模型</param>
        ///// <returns></returns>
        //public static string Post<T>(string requestUri, T model, IDictionary<string, string> parameters = null)
        //{
        //    HttpClient client = new HttpClient();
        //    if (parameters != null && parameters.Count > 0)
        //    {
        //        foreach (var item in parameters)
        //        {
        //            client.DefaultRequestHeaders.Add(item.Key, item.Value);
        //        }
        //    }
            
        //    HttpResponseMessage message = client.PostAsJsonAsync(requestUri, model).Result;client.po
        //    if (message.IsSuccessStatusCode)
        //    {
        //        return message.Content.ReadAsStringAsync().Result;
        //    }
        //    return null;

        //}
        /// <summary>
        /// 使用Post方法获取字符串结果
        /// </summary>
        /// <returns></returns>
        public static string HttpPost(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null)
        {
            return HttpPost(url, cookieContainer, formData, encoding, 12000);
        }
        public static string HttpPost(string url, CookieContainer cookieContainer = null, Dictionary<string, string> formData = null, Encoding encoding = null, int timeout = 20)
        {
            string dataString = GetQueryString(formData);
            var formDataBytes = formData == null ? new byte[0] : (encoding == null ? Encoding.UTF8.GetBytes(dataString) : encoding.GetBytes(dataString));
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(formDataBytes, 0, formDataBytes.Length);
                ms.Seek(0, SeekOrigin.Begin);//设置指针读取位置
                string ret = HttpPost(url, cookieContainer, ms, false, encoding, timeout);
                return ret;
            }
        }
        /// <summary>
        /// 使用Post方法获取字符串结果
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpPost(string url, CookieContainer cookieContainer = null, string fileName = null, Encoding encoding = null)
        {
            //读取文件
            FileStream fileStream = null;
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                fileStream = new FileStream(fileName, FileMode.Open);
            }
            return HttpPost(url, cookieContainer, fileStream, true, encoding);
        }

        /// <summary>
        /// 使用Post方法获取字符串结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="postStream"></param>
        /// <param name="isFile">postStreams是否是文件流</param>
        /// <returns></returns>
        public static string HttpPost(string url, CookieContainer cookieContainer = null, Stream postStream = null, bool isFile = false, Encoding encoding = null, int timeout = 1200000)
        {
            ServicePointManager.DefaultConnectionLimit = 200;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postStream != null ? postStream.Length : 0;
            request.Timeout = timeout;
            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }
            if (postStream != null)
            {
                //postStream.Position = 0;

                //上传文件流
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }

                postStream.Close();//关闭文件访问
            }
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();

                if (cookieContainer != null)
                {
                    response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
                }

                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(responseStream, encoding ?? Encoding.GetEncoding("utf-8")))
                    {
                        string retString = myStreamReader.ReadToEnd();
                        return retString;
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (request != null)
                {
                    request.Abort();
                }
            }


        }



        /// <summary>
        /// 请求是否发起自微信客户端的浏览器
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool IsWeixinClientRequest(this HttpContext httpContext)
        {
            return !string.IsNullOrEmpty(httpContext.Request.UserAgent) &&
                   httpContext.Request.UserAgent.Contains("MicroMessenger");
        }

        /// <summary>
        /// 组装QueryString的方法
        /// 参数之间用&连接，首位没有符号，如：a=1&b=2&c=3
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public static string GetQueryString(this Dictionary<string, string> formData)
        {
            if (formData == null || formData.Count == 0)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            var i = 0;
            foreach (var kv in formData)
            {
                i++;
                sb.AppendFormat("{0}={1}", kv.Key, kv.Value);
                if (i < formData.Count)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 封装System.Web.HttpUtility.HtmlEncode
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string html)
        {
            return System.Web.HttpUtility.HtmlEncode(html);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.HtmlDecode
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string html)
        {
            return System.Web.HttpUtility.HtmlDecode(html);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.UrlEncode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(this string url)
        {
            return System.Web.HttpUtility.UrlEncode(url);
        }
        /// <summary>
        /// 封装System.Web.HttpUtility.UrlDecode
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(this string url)
        {
            return System.Web.HttpUtility.UrlDecode(url);
        }
    }
}
