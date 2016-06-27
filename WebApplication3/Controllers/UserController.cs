using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebApplication3.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }
        #region 微站登录
        public ActionResult loginfromwezhan() {

            string result = "";
            if (IValidateCode_Get() && !string.IsNullOrEmpty(Request.QueryString["accesstoken"])
                && HttpContext.Request.UrlReferrer!=null
                )
            {
                //微站二级域名
                string wezhanurl = "http://" + HttpContext.Request.UrlReferrer.Authority;
                string url = wezhanurl +
                    "/api/IAuthApi/validateAccesstoken?accesstoken=" + Request.QueryString["accesstoken"] + "&applicationkey=" + Request.QueryString["applicationkey"];
                string accesstoken = Request.QueryString["accesstoken"];
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                result = wc.DownloadString(url);
            }


            return Content(result);
        }

        public bool IValidateCode_Get()
        {
            string _validateTokenMessage = "";
            if (string.IsNullOrEmpty(Request.QueryString["tokenStr"]))
            {
                return false;
            }
            try
            {
                var token = Request.QueryString["tokenStr"];
                string ApplicationKey = Request.QueryString["applicationkey"];

                List<string> queryKeyValues = new List<string>();
                queryKeyValues.Add("yunmeng123456");
                queryKeyValues.Add(Request.QueryString["email"]);
                //queryKeyValues.Add(Request.QueryString["randomstr"]);
                queryKeyValues.Add(Request.QueryString["strtimestamp"]);
                queryKeyValues.Add(ApplicationKey);

                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(Request.QueryString["strTimestamp"] + "0000");
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime date = dtStart.Add(toNow);


                if (date > DateTime.Now && token == GetToken(queryKeyValues))
                {
                    return true;
                }
                _validateTokenMessage = "验证信息错";
                return false;
            }
            catch (Exception)
            {
                _validateTokenMessage = "验证信息服务器错误";
            }
            return false;
        }

        private string GetToken(List<string> queryKeyValues)
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

        #endregion
    }
}