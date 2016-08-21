using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program测试访问登录接口
    {
        static void Main(string[] args)
        {
            //FlipCloud();
            SsologinForMail();
            Console.Read();
        }

        public static void SsologinForMail() {
            string username = "zengjw@andni.cn";//"xuna@clouddream.net";"123456@qq.com";
            string url2 = "http://localhost:9090/customer/ssologin?username=" + username;

            string _securityKey = "yunmeng123";
            TimeSpan ts = DateTime.Now.AddMinutes(30) - new DateTime(1970, 1, 1);
            //生成Token的数据
            List<string> queryKeyValues = new List<string>();
            queryKeyValues.Add(ts.TotalSeconds.ToString());
            queryKeyValues.Add(_securityKey);
            queryKeyValues.Add(username);
            
            var token = GetToken(queryKeyValues);

            string url = url2 + "&timestamp=" + ts.TotalSeconds + "&token=" + token + "&redirecturl=" + System.Web.HttpUtility.UrlEncode("/ap");
            Console.Read();
        }
        public static void requestLoginGet() {
            TimeSpan ts2 = DateTime.Now.AddMinutes(10) - new DateTime(1970, 1, 1);
            string url2 = "http://192.168.199.222:8012/api/IAuthApi/loginget?email=123456@qq.com&password=111111";
            //string content = HttpGet(url,null,null);

            string ApplicationKey = "yunmengapp", _securityKey = "yunmeng_123456";
            string randomstr = Guid.NewGuid().ToString();
            //生成Token的数据
            List<string> queryKeyValues = new List<string>();
            queryKeyValues.Add(_securityKey);
            queryKeyValues.Add("123456@qq.com");
            queryKeyValues.Add(randomstr);
            queryKeyValues.Add(Convert.ToInt64(ts2.TotalMilliseconds).ToString());
            queryKeyValues.Add(ApplicationKey);
            var token = GetToken(queryKeyValues);

            url2 += string.Format("&randomstr={0}&strtimestamp={1}&tokenstr={2}&applicationkey={3}",
                randomstr, Convert.ToInt64(ts2.TotalMilliseconds).ToString(), token, ApplicationKey);

            string content = HttpGet(url2, null);
        }

        public static void requestLoginPost() { 

            string url2 = "http://192.168.199.222:8012/api/IAuthApi/login/";

            string ApplicationKey = "yunmengapp", _securityKey = "yunmeng_123456";
            string randomstr = Guid.NewGuid().ToString();
            TimeSpan ts = DateTime.Now.AddMinutes(10) - new DateTime(1970, 1, 1); 
            //生成Token的数据
            List<string> queryKeyValues = new List<string>();
            queryKeyValues.Add(_securityKey);
            queryKeyValues.Add("123456@qq.com");
            queryKeyValues.Add(randomstr);
            queryKeyValues.Add(Convert.ToInt64(ts.TotalMilliseconds).ToString());
            queryKeyValues.Add(ApplicationKey);
            var token = GetToken(queryKeyValues);

            //用RequestHeaders来传的参数 
            string customerAuthKey = ApplicationKey + ":" + randomstr + ":" + Convert.ToInt64(ts.TotalMilliseconds).ToString() + ":" + token;
            System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
            var model = json.Serialize(new { email = "123456@qq.com", password = "111111" });

            string content = HttpPost(url2, Encoding.UTF8.GetBytes(model), customerAuthKey);
        }

        public static void requestGetUserInfo(string _token) {
            TimeSpan ts2 = DateTime.Now.AddMinutes(10) - new DateTime(1970, 1, 1);
            string url2 = "http://192.168.199.222:8012/api/IAuthApi/GetUserInfo?accesstoken=" + _token + "&email=123456@qq.com";
            //string content = HttpGet(url,null,null);

            string ApplicationKey = "yunmengapp", _securityKey = "yunmeng_123456";
            string randomstr = Guid.NewGuid().ToString();
            //生成Token的数据
            List<string> queryKeyValues = new List<string>();
            queryKeyValues.Add(_securityKey);
            queryKeyValues.Add("123456@qq.com");
            queryKeyValues.Add(randomstr);
            queryKeyValues.Add(Convert.ToInt64(ts2.TotalMilliseconds).ToString());
            queryKeyValues.Add(ApplicationKey);
            var token = GetToken(queryKeyValues);

            url2 += string.Format("&randomstr={0}&strtimestamp={1}&tokenstr={2}&applicationkey={3}",
                randomstr, Convert.ToInt64(ts2.TotalMilliseconds).ToString(), token, ApplicationKey);

            string content = HttpGet(url2, null);
        }
        public static string requestRefresh()
        {
            TimeSpan ts2 = DateTime.Now.AddMinutes(10) - new DateTime(1970, 1, 1);
            string url2 = "http://192.168.199.222:8012/api/IAuthApi/RefreshAccessToken?refreshtoken=db26edc8-aae9-43b9-b46d-fb788f760fe8&applicationkey=yunmengapp";
            //string content = HttpGet(url,null,null);

            

            string content = HttpGet(url2, null);
            return content;
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

        private static string GetToken(List<string> queryKeyValues)
        {
            string[] queryKeyValueArr = queryKeyValues.ToArray();
            string queryKeyValueStr = string.Join("", queryKeyValueArr);

            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(Encoding.UTF8.GetBytes(queryKeyValueStr));
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            queryKeyValueStr = strBuilder.ToString();

            return queryKeyValueStr;
        }
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
        private static string HttpPost(string url, byte[] postStream, string authorization, Encoding encoding = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "POST";
            request.Headers.Add("Custom-Auth-Key", authorization);

            request.ContentType = "application/json;charset=utf-8";
            request.Accept = "application/json";

            request.Timeout = 60 * 2 * 1000; // 同步接口 调用时间2分钟
            request.ServicePoint.Expect100Continue = false;

            HttpWebResponse response = null;
            try
            {
                postStream = postStream ?? new byte[] { };
                request.ContentLength = postStream.Length;
                var requestStream = request.GetRequestStream();
                requestStream.Write(postStream, 0, postStream.Length);
                requestStream.Close();
                response = (HttpWebResponse)request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (StreamReader myStreamReader = new StreamReader(responseStream, encoding ?? Encoding.UTF8))
                        {
                            return myStreamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                request.Abort();
            }
            return "";
        }

        public static void FlipCloud() {
            string url = "http://bc.clouddream.net/api/webapi/advertiseservice/GetSiteTypeInfo?siteid=1013765";
            string content = HttpGet(url, GetDomainBindParameters());
            return;
        }
        public static IDictionary<string, string> GetDomainBindParameters()
        {
            TimeSpan ts1 = DateTime.Now.AddMinutes(10) - new DateTime(1970, 1, 1);

            List<string> queryKeyValues = new List<string>();
            queryKeyValues.Add(Convert.ToInt64(ts1.TotalSeconds).ToString());
            queryKeyValues.Add("yunmeng123456");

            var token = GetToken(queryKeyValues);

            IDictionary<string, string> parameters = new Dictionary<string, string>();
            string customerAuthKey = Convert.ToInt64(ts1.TotalSeconds).ToString() + ":" + token;
            parameters.Add("Custom-Auth-Key", customerAuthKey);

            return parameters;
        }
        // 对称加密帮助类
        public class CryptoHelper
        {

            // 对称加密算法提供器
            private ICryptoTransform encryptor;     // 加密器对象
            private ICryptoTransform decryptor;     // 解密器对象
            private const int BufferSize = 1024;

            public CryptoHelper(string algorithmName, string key)
            {
                SymmetricAlgorithm provider = SymmetricAlgorithm.Create(algorithmName);
                provider.Key = Encoding.UTF8.GetBytes(key);
                provider.IV = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

                encryptor = provider.CreateEncryptor();
                decryptor = provider.CreateDecryptor();
            }

            public CryptoHelper(string key) : this("TripleDES", key) { }

            // 加密算法
            public string Encrypt(string clearText)
            {
                // 创建明文流
                byte[] clearBuffer = Encoding.UTF8.GetBytes(clearText);
                MemoryStream clearStream = new MemoryStream(clearBuffer);

                // 创建空的密文流
                MemoryStream encryptedStream = new MemoryStream();

                CryptoStream cryptoStream =
                    new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write);

                // 将明文流写入到buffer中
                // 将buffer中的数据写入到cryptoStream中
                int bytesRead = 0;
                byte[] buffer = new byte[BufferSize];
                do
                {
                    bytesRead = clearStream.Read(buffer, 0, BufferSize);
                    cryptoStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                cryptoStream.FlushFinalBlock();

                // 获取加密后的文本
                buffer = encryptedStream.ToArray();
                string encryptedText = Convert.ToBase64String(buffer);
                return encryptedText;
            }

            // 解密算法
            public string Decrypt(string encryptedText)
            {
                byte[] encryptedBuffer = Convert.FromBase64String(encryptedText);
                Stream encryptedStream = new MemoryStream(encryptedBuffer);

                MemoryStream clearStream = new MemoryStream();
                CryptoStream cryptoStream =
                    new CryptoStream(encryptedStream, decryptor, CryptoStreamMode.Read);

                int bytesRead = 0;
                byte[] buffer = new byte[BufferSize];

                do
                {
                    bytesRead = cryptoStream.Read(buffer, 0, BufferSize);
                    clearStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                buffer = clearStream.GetBuffer();
                string clearText =
                    Encoding.UTF8.GetString(buffer, 0, (int)clearStream.Length);

                return clearText;
            }

            public static string Encrypt(string clearText, string key)
            {
                CryptoHelper helper = new CryptoHelper(key);
                return helper.Encrypt(clearText);
            }

            public static string Decrypt(string encryptedText, string key)
            {
                CryptoHelper helper = new CryptoHelper(key);
                return helper.Decrypt(encryptedText);
            }
        }

        #region 返回的 models
        /// <summary>
        /// 请求成功, 2字头
        /// 请求错误, 4字头
        /// 服务器错误, 5字头
        /// </summary>
        public enum ErrorEnum
        {
            OK = 200,
            WrongUserPassword = 2031,
            NonExistUser = 2032,
            NonExistDomain = 2033,
            UserIsLocked = 2034,
            UserIsDeleted = 2034,

            /// <summary>
            /// 参数错误
            /// </summary>
            WrongParameter = 400,

            /// <summary>
            /// 未通过验证
            /// </summary>
            Unauthorized = 401,

            /// <summary>
            /// 服务器端错误
            /// </summary>
            InternalServerError = 500
        }
        public class Error
        {
            public ErrorEnum enum_ErrorNum { get; set; }
            public string Message { get; set; }
        }

        public class AccessToken
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }

            public int expire_in { get; set; }
        }

        public class Model_Login
        {
            public bool IsSuccess { get; set; }
            public BasicUserInfo BasicUserInfo { get; set; }
            public Error Error { get; set; }

            public AccessToken accesstoken { get; set; }
        }

        /// <summary>
        /// 信息类
        /// </summary>
        public class BasicUserInfo
        {
            public int UserID { get; set; }
            public Guid UserGuid { get; set; }
            public string UserName { get; set; }
            public string UserEmail { get; set; }
            public bool IsSystemAccount { get; set; }

            public DateTime? LastLoginDatetime { get; set; }

            public DateTime? LastActiveDatetime { get; set; }

            public int SiteID { get; set; }
            public string domain { get; set; }
        }

        #endregion
    }
}
