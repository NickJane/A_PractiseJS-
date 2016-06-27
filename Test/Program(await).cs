using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void readxml() {
            string xmls = @"C:/搜狗高速下载/VmHostConfig.xml";
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(xmls);
            string skuids = "";
            foreach (System.Xml.XmlNode item in doc.SelectNodes("//SkuItem"))
            {
                skuids +=item.Attributes["SkuId"].Value+ ",";
            }
            Console.WriteLine(skuids);
            Console.Read();
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
        public static string getUrlParameters() {
            TimeSpan ts1 = DateTime.UtcNow.AddSeconds(600) - new DateTime(1970, 1, 1);

            List<string> queryKeyValues = new List<string>();
            queryKeyValues.Add(Convert.ToInt64(ts1.TotalSeconds).ToString());//1时间戳
            queryKeyValues.Add("yunmeng123");//secretkey
            queryKeyValues.Add("123456@qq.com");//需要一键登录的用户名
            var token = GetToken(queryKeyValues);

            string url = string.Format("username={0}&timestamp={1}&token={2}&redirecturl={3}", "123456@qq.com", Convert.ToInt64(ts1.TotalSeconds), token,
                System.Web.HttpUtility.UrlEncode("http://192.168.199.226:8012/apv2"));
            return url;
        }
        void Main(string[] args)
        {
            Bird bird = new Bird();
            Chicken chicken = new Chicken();
            Bird c2 = new Chicken();
            c2.ShowType()
                ;
            getUrlParameters();
            //readxml();
            //// 异步方式
            //Console.WriteLine("\n异步方式测试开始！main线程id是{0}",System.Threading.Thread.CurrentThread.ManagedThreadId);
            ////AsyncMethod(0);
            //AsyncMethod_taks(10);
            //Console.WriteLine("异步方式测试结束！");
            Console.ReadKey();
        }

        // 异步操作
        private static async void AsyncMethod(int input)
        {
            Console.WriteLine("进入异步操作！线程id是{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            var result = await AsyncWork(input);
            Console.WriteLine("最终结果{0}, 线程ID是{1}", result, System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("退出异步操作！");
        }

        // 模拟耗时操作（异步方法）
        private static async Task<int> AsyncWork(int val)
        {
            await Task.Delay(2000);
            for (int i = 0; i < 5; ++i)
            {
                Console.WriteLine("耗时操作{0}, 线程id是 {1}", i, System.Threading.Thread.CurrentThread.ManagedThreadId);
                val++;
            }
            return val;
        }

        // 异步操作
        private static async void AsyncMethod_taks(int input)
        {
            Console.WriteLine("进入异步操作！");
            var result = await Task.Factory.StartNew((Func<object, int>)SyncWork2, input);
            Console.WriteLine("最终结果{0}", result);
            Console.WriteLine("退出异步操作！");
        }

        // 模拟耗时操作（同步方法）
        private static int SyncWork2(object input)
        {
            int val = (int)input;
            for (int i = 0; i < 5; ++i)
            {
                Console.WriteLine("耗时操作{0}, 线程id是 {1}", i, System.Threading.Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(3000);
                val++;
            }
            return val;
        }
    }
}
