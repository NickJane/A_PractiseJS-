using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using BaiduBce.Services.Bos.Model;
using UpdateBos.helper;
using log4net;
using System.Xml;
using Newtonsoft.Json;
namespace UpdateBos
{
    class Program
    {
        /*
         程序说明
         <add key="UpdateDate" value="2015-12-06"/>
         <add key="rootPath" value="c:\publish\test"/>
         <add key="Yun_key_pre" value="testKey"/>
         
         
         
         */
        #region 内部帮助方法
        public static XmlAttribute CreateAttribute(XmlNode node, string attributeName, string value)
        {
            try
            {
                XmlDocument doc = node.OwnerDocument;
                XmlAttribute attr = null;
                attr = doc.CreateAttribute(attributeName);
                attr.Value = value;
                node.Attributes.SetNamedItem(attr);
                return attr;
            }
            catch (Exception err)
            {
                string desc = err.Message;
                return null;
            }
        } 
        static string getCurrentDay() { 
            string path= Directory.GetCurrentDirectory()+"\\successdate.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            var root = doc.SelectSingleNode("root");
            var days = root.SelectNodes("day");
            var flag = true;
            var i = 1;
            while (flag) {
                var last = days[days.Count - i];
                if (last.ChildNodes.Count == 0)
                {
                    flag = false;
                    return last.Attributes["id"].Value;
                }
            }
            return "";
        }
        static void writeCurrentRunDay() {
            string path = Directory.GetCurrentDirectory() + "\\successdate.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            var root = doc.SelectSingleNode("root"); 
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var days = root.SelectNodes("day");
            var flag = false;
            XmlNode current;
            for (int i = 0; i < days.Count; i++)
            {
                if (days[i].Attributes["id"].Value == today)
                {    
                    flag = true;
                    current = days[i];
                }
            }
            if (!flag) {
                var node = doc.CreateElement("day");
                node.Attributes.Append(CreateAttribute(node, "id", today));
                current = root.AppendChild(node);
                doc.Save(path);
            }

        }
        static void deletecurrentday() {
            string path = Directory.GetCurrentDirectory() + "\\successdate.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            var root = doc.SelectSingleNode("root");
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var days = root.SelectNodes("day");

            XmlNode current;
            for (int i = 0; i < days.Count; i++)
            {
                if (days[i].Attributes["id"].Value == today)
                {
                    current = days[i];
                    root.RemoveChild(current);
                    doc.Save(path);
                }
            }
        }

        /// <summary>
        /// 将阿里云上的东西保存到本地
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        private static string CreateFile(Aliyun.OpenServices.OpenStorageService.OssObject obj)
        {
            var temparr = obj.Key.Split('/');
            var filename = temparr[temparr.Length - 1];//把key按/分组, 获得最后一个则是文件名

            var templist=temparr.ToList();
            templist.RemoveAt(temparr.Length-1);
            var key_directory = "";
            templist.ForEach(x => key_directory += x + "/");// 获得除了文件名之外的字符串, 用来组成本地的路径名

            var fulldirectory = Environment.CurrentDirectory + "/" + key_directory;//本地路径
            if (!Directory.Exists(fulldirectory)) Directory.CreateDirectory(fulldirectory);

            var fullname = fulldirectory + filename;//本地文件名全名
            
            int len = (int)obj.Metadata.ContentLength;
            byte[] b = new byte[len];
            using (BinaryReader reader = new BinaryReader(obj.Content))
            {
                b = reader.ReadBytes(b.Length);
                if (obj.Metadata.ContentLength == b.Length)
                {
                    using (var fileStream = File.Create(fullname))
                    {
                        fileStream.Write(b, 0, len);
                        fileStream.Close();
                    }
                }
            }
            return fullname;
        }
        #endregion
        static void Main(string[] args)
        {
            
            //配置文件

            DateTime updateDate = Convert.ToDateTime( getCurrentDay());//Convert.ToDateTime(System.Configuration.ConfigurationSettings.AppSettings[0]);
            string FolderPath = System.Configuration.ConfigurationSettings.AppSettings[0];
            string Pre_key = System.Configuration.ConfigurationSettings.AppSettings[1];//云盘上的key根目录

            OssConfig config = OssConfigMgr.GetOneConfig();
            BosOpenStorageService bos = new BosOpenStorageService(config);
            OssConfig aliConfig = OssConfigMgr.GetConfig(2);
            AliyunOpenStorageService aos = new AliyunOpenStorageService(aliConfig);

            OssConfig ali_hk_Config = OssConfigMgr.GetConfig(3);
            AliyunOpenStorageService hk_aos = new AliyunOpenStorageService(ali_hk_Config);

            OssConfig ali_us_Config = OssConfigMgr.GetConfig(4);
            AliyunOpenStorageService us_aos = new AliyunOpenStorageService(ali_us_Config);

            try
            {
                var flag1 = true; var which = "";
                while (flag1)
                {
                    Console.WriteLine("\n\n\n欢迎使用云梦云端帮助工具");
                    Console.WriteLine("请输入数字选择程序: \n1上传(支持百度) \n2删除(支持百度) \n3模版文件迁移(从阿里云到百度云) \n4整站文件多平台迁移(待完成) \n5模板文件迁移(从阿里香港云到百度) \n6 阿里云文件迁移(香港云到美国云)");
                    which = Console.ReadLine();
                    if (which != "1" && which != "2" && which != "3" && which != "5" && which != "6" && which != "temp")
                        continue;

                    Console.WriteLine(""); Console.WriteLine("");
                    switch (which)
                    {
                        case "1":
                            updateDate = RunUpload(updateDate, FolderPath, Pre_key, bos);
                            break;
                        case "2":
                            delete(bos);
                            break;
                        case "3":
                            TemplateTransfer(bos, aos);
                            break;
                        case "5":
                            TemplateTransfer(bos, hk_aos);
                            break;
                        case "6":
                            TemplateTransfer_forAliyun(hk_aos, us_aos);
                            break;
                        case "temp":
                            //TempFunction(us_aos);
                            VisitUSA();
                            break;
                    }
                }
            }
            catch (Exception ex) {
                LogHelper.CreateErrorLogTxt(ex.Message);
            }

            Console.Read();
        }

        private static void delete(BosOpenStorageService bos)
        {
            Console.WriteLine("删除程序启动..........");
            Console.WriteLine("请输入要删除的云文件key,key代表百度云上的文件夹名称");
            Console.WriteLine("例如: 输入testKey, 将会列出该文件夹下所有的文件(包含子孙文件夹内文件)");
            Console.WriteLine("输入111回到上层菜单");
            var flag = true;
            List<BosObjectSummary> list=new List<BosObjectSummary>();
            string key = "";
            while (flag)
            {
                key = Console.ReadLine();
                if (key == "111") 
                    return;
                list = bos.ListObjects(bos.OssConfig.BucketName, key + "/");
                if (list.Count == 0)
                {
                    Console.WriteLine("该key下没有任何文件, 请重新输入, 输入111则退出");
                    continue;
                }
                Console.WriteLine("正在列出key下所有的文件");
                for (int i = 0; i < list.Count; i++)
                {
                    var obj = list[i];
                    Console.WriteLine("{0}-------{1}", (i + 1), obj.Key);
                }
                flag = false;
            }

            while (true)
            {
                Console.WriteLine("\n 确定删除请按y");
                var yes = Console.ReadLine();
                if (yes == "y") {
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            bos.deleteObject(bos.OssConfig.BucketName, list[i].Key);
                            Console.WriteLine("删除成功{0}---{1}", i + 1, list[i].Key);
                        }
                        catch (Exception ex) {
                            LogHelper.CreateErrorLogTxt(string.Format("{2}..删除文件失败 {0},  {1}", key, ex.Message,DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
                        }
                    }
                    break;
                }

            }
            list = bos.ListObjects(bos.OssConfig.BucketName, key + "/");
            if (list.Count == 0)
            {
                Console.WriteLine("\n\n 全部删除成功");
            }
            else {
                Console.WriteLine("\n\n 删除失败, 仍有下列文件未删除");
                for (int i = 0; i < list.Count; i++)
                {
                    var obj = list[i];
                    Console.WriteLine("{0}-------{1}", (i + 1), obj.Key);
                }
            }
            Console.ReadLine();
        }

        private static DateTime RunUpload(DateTime updateDate, string FolderPath, string Pre_key, BosOpenStorageService bos)
        {
            #region 读取用户输入
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("使用默认更新的时间点 {0} 则输入y, 或者输入时间,格式为:2015-12-05", updateDate.ToString("yyyy-MM-dd"));
                string P_time = Console.ReadLine();
                if (P_time.ToLower() != "y")
                {
                    DateTime temptime;
                    if (DateTime.TryParse(P_time, out temptime))
                    {
                        flag = false;
                        updateDate = temptime;
                    }
                }
                if (P_time.ToLower() == "y")
                    flag = false;
            }
            #endregion

            Console.WriteLine("启动完毕, 更换文件时间节点是 " + updateDate.ToLongDateString());
            writeCurrentRunDay();//把updateDate设置为今天

            LogHelper.CreateErrorLogTxt(string.Format("-----------------------{0}-开始发布----------------------", DateTime.Now.ToLongDateString()));

            AllFiles allFiles = new AllFiles();
            List<String> list = allFiles.FindFile(FolderPath, updateDate);//所有文件的物理地址
            List<String> list2 = new List<string>();//文件上传到云盘上的key集合 
            #region 文件上传到云盘上的key集合
            foreach (var item in list)
            {
                //Console.WriteLine(item);
                string tempItem = item;
                tempItem = item.Replace(FolderPath, "").Replace('\\', '/');
                tempItem = Pre_key + tempItem;
                list2.Add(tempItem);
            }
            #endregion

            LogHelper.CreateErrorLogTxt(string.Format("读取完毕所有的文件, 共计 {0}", list.Count));

            var isError = false;
            for (int i = 0; i < list2.Count; i++)
            {
                string key = list2[i];
                #region 上传到云
                using (FileStream fs = new FileStream(list[i], FileMode.Open))
                {
                    //获得后缀名
                    var temp = list[i].Split('.');
                    string ext = temp.Length > 1 ? temp[temp.Length - 1] : "";
                    try
                    {
                        //if (i > 28)
                        //    throw new Exception();
                        //---------------上传到bos
                        ObjectMetadata metadata = new ObjectMetadata();
                        metadata.ContentLength = fs.Length;
                        metadata.ContentType = FileContentType.GetMimeType(ext);
                        bos.PutObject(bos.OssConfig.BucketName, key, fs, metadata);

                        Console.WriteLine("完成上传{0}----{1}", i, key);
                        File.Delete(list[i]);//成功之后就删除

                    }
                    catch (Exception ex)
                    {
                        isError = true;
                        Console.WriteLine("处理{0}遇到问题, 代码是{1}, 当前进度{2}/{3}, 继续请按y", key, ex.Message, i + 1, list2.Count);
                        var ifcontinue = Console.ReadLine();
                        if (ifcontinue.ToLower().Trim() != "y")
                            break;

                        LogHelper.CreateErrorLogTxt(string.Format("失败 {0},  {1}", key, ex.Message));
                    }
                }
                #endregion
            }
            LogHelper.CreateErrorLogTxt(string.Format("-----------------------{0}-完成----------------------\n\n\n", DateTime.Now.ToLongDateString()));
            if (!isError)
                Console.WriteLine("任务完成, 没有出现任何问题,");
            else
            {
                Console.WriteLine("上传出现问题, 请查看日志");
                deletecurrentday();//重置上次成功的上传时间
            }
            return updateDate;
        }

        private static void TemplateTransfer(BosOpenStorageService bos, AliyunOpenStorageService aos)
        {
            Console.WriteLine("目前程序支持阿里云迁移到百度云");
            
            int siteid = 0, targetSiteID=0;
            
            #region 解析siteid
            while (true)
            {
                Console.WriteLine("请输入阿里云的SiteID");
                var sourceSiteID = Console.ReadLine();
                if (int.TryParse(sourceSiteID, out siteid)) {
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("请输入百度云的SiteID");
                var baiduyunsiteid = Console.ReadLine();
                if (int.TryParse(baiduyunsiteid, out targetSiteID))
                {
                    break;
                }
            }
            //Console.WriteLine("解析siteid中......");
            //var isError = true;
            //while (isError)
            //{
            //    Console.WriteLine("请输入源模板的二级域名, 格式为 889117024.wezhan.cn");
            //    var domain = Console.ReadLine();
            //    try
            //    {
            //        domain = "http://" + domain + "/home/index";
            //        var htmls = RequestUtility.HttpGet(domain, null);

            //        string pattern = "content/sitefiles/\\d{2,7}/images";
            //        System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            //        var temp = re.Match(htmls);
            //        siteid = int.Parse(temp.Value.Replace("content/sitefiles/", "").Replace("/images", ""));
            //        Console.WriteLine("解析完毕, 即将列出所有的问题......");
            //        isError = false;//跳出循环
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("出现错误, {0}", ex.Message);
            //        //Console.WriteLine("程序正在尝试二次解析siteid");
            //        isError = true;
            //    }
            //}
            #endregion

            var list = new List<string>();//获取云端所有文件
            var uplistkey = new List<string>();//list副本, 上传的时候读取 
            var uplistFullName = new List<string>();
            #region
            Console.WriteLine("开始从阿里云读取文件");System.Threading.Thread.Sleep(1000);
            var index = 1;
            Aliyun.OpenServices.OpenStorageService.ObjectListing result = null;
            string nextMarker = string.Empty;
            do
            {
                var listObjectsRequest = new Aliyun.OpenServices.OpenStorageService.ListObjectsRequest(aos.OssConfig.BucketName)
                {
                    Marker = nextMarker,
                    MaxKeys = 100
                    ,
                    Prefix = "content/sitefiles/" + siteid.ToString() + "/"//只有图片的话不需要sitefiles分组
                };
                result = aos.ListObjects(listObjectsRequest);

                Console.WriteLine("File:");
                foreach (var summary in result.ObjectSummaries)
                {
                    var tempkey = summary.Key;
                    var tempkeys = tempkey.Split('/');
                    var filename = tempkeys[tempkeys.Length - 1];
                    if (filename.Split('.').Length != 2) //如果没有后缀名. 那就不管了
                        continue;
                    list.Add(summary.Key);
                    Console.WriteLine("{1}--Name:{0}", summary.Key, index);
                    index++;
                }
                nextMarker = result.NextMarker;

            } while (result.IsTruncated);//(false);// 
            Console.WriteLine("\n\n 共{0}个文件, 现在开始下载...", list.Count);
            uplistkey = list.ToArray().ToList();
            #endregion

            #region 下载到本地 目标是程序所在文件夹
            //var errorlist = new List<string>();
            //var latestError = "";
            
            //for (int i = 0; i < list.Count; i++)
            //{
            //    try
            //    {
            //        using (Aliyun.OpenServices.OpenStorageService.OssObject obj = aos.GetObject(aos.OssConfig.BucketName, list[i]))
            //        {
            //            CreateFile(obj);
            //            Console.WriteLine("{0}下载成功--{1}", i + 1, obj.Key);
            //        }
            //    }catch(Exception ex){
            //        errorlist.Add(list[i]);
            //        latestError = ex.Message;
            //    }
            //}
            System.Threading.Thread.Sleep(2000);
            index = 1;
            while (list.Count > 0)
            {
                var curr = list[0];
                try
                {
                    using (Aliyun.OpenServices.OpenStorageService.OssObject obj = aos.GetObject(aos.OssConfig.BucketName, curr))
                    {
                        uplistFullName.Add(CreateFile(obj));
                        Console.WriteLine("{0}下载成功--{1}", index, obj.Key);
                        index++;
                        list.Remove(curr);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("出现错误, 信息是{0}", ex.Message);
                    Console.WriteLine("重试中......");
                }
            }

            Console.WriteLine("下载完成");
            #endregion

            Console.WriteLine("开始上传到百度云\n\n");
            #region 上传到百度云
            index = 1;
            while (uplistkey.Count > 0)
            {
                var sourceKey = uplistkey[0];//下载到本地的keys
                var targetKey = uplistkey[0];//上传到百度的keys
                #region 处理key
                var keys = targetKey.Split('/');
                keys[2] = targetSiteID.ToString();
                targetKey = "";
                keys.ToList().ForEach(x => targetKey += x + "/");
                targetKey = targetKey.Remove(targetKey.Length - 1);
                #endregion
                //var fullname = Environment.CurrentDirectory + "/" + key;
                using (FileStream fs = new FileStream(sourceKey, FileMode.Open))
                {
                    //获得后缀名
                    var temp = targetKey.Split('.');
                    string ext = temp.Length > 1 ? temp[temp.Length - 1] : "";
                    try
                    {
                        ObjectMetadata metadata = new ObjectMetadata();
                        metadata.ContentLength = fs.Length;
                        metadata.ContentType = FileContentType.GetMimeType(ext);
                        bos.PutObject(bos.OssConfig.BucketName, targetKey, fs, metadata);

                        Console.WriteLine("完成上传{0}----{1}", index, targetKey);
                        File.Delete(sourceKey);//成功之后就删除本地sourcekey
                        uplistkey.Remove(sourceKey);
                        index++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("处理{0}遇到问题, 代码是{1}, 当前进度{2}/{3}", sourceKey, ex.Message, index, uplistkey.Count);
                        Console.WriteLine("重试中");
                    }
                }
            }
            #endregion


            Console.WriteLine("\n\n完成上传");

            Console.ReadLine();
        }

        private static void TemplateTransfer_forAliyun( AliyunOpenStorageService aos, AliyunOpenStorageService aosTarget)
        {
            var rootpath = Directory.GetCurrentDirectory();
            var sourceFile = rootpath + "//app_data//Transfer_Ali_SourceSiteIDs.txt";
            var FinishedIdsFile = rootpath + "//app_data//Transfer_Ali_FinishedIDs.txt";
            var ErrorFile = rootpath + "//app_data//Transfer_Ali_Errors.txt";
            Console.WriteLine("\n\n欢迎使用阿里云站点文件迁移工具.");
            if (!File.Exists(sourceFile))
            {
                Console.WriteLine("未能找到Transfer_Ali_SourceSiteIDs.txt(源头云需要迁移的siteid集合文件)");
                return;
            }
            //本次需要上传的文件里面的id集合
            string[] sourceids = File.ReadAllText(sourceFile).Split(',');
            for (int i = 0; i < sourceids.Length; i++)
            {
                string tempsorceid = sourceids[i].Replace("\r","").Replace("\n","");
                sourceids[i] = tempsorceid;
            }
            Console.WriteLine("\n文件中共有{0}个站点, 系统将会从日志中匹配未迁移过的站点.", sourceids.Length);
            
           
            
            var list = new List<string>();//获取云端所有文件, 并在下载的时候下载成功一个移除一个项目
            var uplistkey = new List<string>();//list副本, 上传的时候读取 
            var uplistFullName = new List<string>();
           

            for (int i = 0; i < sourceids.Length; i++)
            {  //json日志文件
                var FinishedList = JsonConvert.DeserializeAnonymousType(File.ReadAllText(FinishedIdsFile),
                new[] { new { SiteID = 0, FinishedDatetime = "" } }).ToList();

                bool thisSiteHasError = false;//该站点在迁移过程全程是否发生过问题
                string ErrorMessage = "";//如果有则用这个记录

                list.Clear(); uplistkey.Clear();//清掉

                int tempsiteid = 0, targetSiteID =0;//香港迁移美国的规则id一致
                if (!int.TryParse(sourceids[i], out tempsiteid)) continue; targetSiteID = tempsiteid;
                var tempobj = FinishedList.Where(x => x.SiteID == tempsiteid).FirstOrDefault();
                if (tempobj!=null)
                    continue;

                #region 开始读取SiteID: {0} ,往list里面写入这个站点所有的文件

                var index = 1;
                Aliyun.OpenServices.OpenStorageService.ObjectListing result = null;
                string nextMarker = string.Empty;
                Console.WriteLine("开始读取SiteID: {0}", tempsiteid);
                try
                {
                    do
                    {
                        #region 往list里面写入这个站点所有的文件key

                        var listObjectsRequest = new Aliyun.OpenServices.OpenStorageService.ListObjectsRequest(aos.OssConfig.BucketName)
                        {
                            Marker = nextMarker,
                            MaxKeys = 100
                            ,
                            Prefix = "content/sitefiles/" + tempsiteid.ToString() + "/"//只有图片的话不需要sitefiles分组
                        };
                        result = aos.ListObjects(listObjectsRequest);

                        
                        foreach (var summary in result.ObjectSummaries)
                        {
                            var tempkey = summary.Key;
                            var tempkeys = tempkey.Split('/');
                            var filename = tempkeys[tempkeys.Length - 1];
                            if (filename.Split('.').Length != 2) //如果没有后缀名. 那就不管了
                                continue;
                            list.Add(summary.Key);
                            //Console.WriteLine("{1}--Name:{0}", summary.Key, index);
                            index++;
                        }
                        nextMarker = result.NextMarker;

                        #endregion

                    } while (result.IsTruncated);//(false);// 
                }
                catch (Exception ex)
                {
                    thisSiteHasError = true;
                    ErrorMessage += "读取云端文件key到集合list的时候出现错误" + ex.Message;
                }
                #region 读取的过程中发生错误的话, 就判断这个站点读取失败, 记录到日志里面, continue循环, 读取下一个站点
                if (thisSiteHasError) {
                    var ErrorList = JsonConvert.DeserializeAnonymousType(File.ReadAllText(ErrorFile),
                    new[] { new { SiteID = 0, FinishedDatetime = "", Message = "" } }).ToList();
                    ErrorList.Add(new { SiteID = tempsiteid, FinishedDatetime = DateTime.Now.ToLongTimeString(), Message = ErrorMessage });
                    string strtemperror = JsonConvert.SerializeObject(ErrorList);
                    File.WriteAllText(ErrorFile, strtemperror);
                    continue;
                }
                #endregion

                Console.WriteLine("\n\n Siteid:{1}, 共{0}个文件, 现在开始下载...", list.Count, tempsiteid);
                uplistkey = list.ToArray().ToList();

                #endregion

                #region 下载到本地 目标是程序所在文件夹
                while (list.Count > 0)
                {
                    var curr = list[0];
                    try
                    {
                        using (Aliyun.OpenServices.OpenStorageService.OssObject obj = aos.GetObject(aos.OssConfig.BucketName, curr))
                        {
                            uplistFullName.Add(CreateFile(obj));
                            list.Remove(curr);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("siteid:{1}, 出现错误, 信息是{0}", ex.Message,tempsiteid);
                        Console.WriteLine("重试中......");
                    }
                }

                Console.WriteLine("下载完成");
                #endregion

                #region 上传到目标云
                Console.WriteLine("开始上传");
                while (uplistkey.Count > 0)
                {
                    bool tempuploaderror = false;//记录上传的过程中是否有错误
                    var sourceKey = uplistkey[0];//下载到本地的keys
                    var targetKey = uplistkey[0];//上传到百度的keys
                    #region 处理key
                    var keys = targetKey.Split('/');
                    keys[2] = targetSiteID.ToString();
                    targetKey = "";
                    keys.ToList().ForEach(x => targetKey += x + "/");
                    targetKey = targetKey.Remove(targetKey.Length - 1);
                    #endregion
                    //var fullname = Environment.CurrentDirectory + "/" + key;
                    using (FileStream fs = new FileStream(sourceKey, FileMode.Open))
                    {
                        //获得后缀名
                        var temp = targetKey.Split('.');
                        string ext = temp.Length > 1 ? temp[temp.Length - 1] : "";
                        try
                        {
                            Aliyun.OpenServices.OpenStorageService.ObjectMetadata metadata = new Aliyun.OpenServices.OpenStorageService.ObjectMetadata();
                            metadata.ContentLength = fs.Length;
                            metadata.ContentType = FileContentType.GetMimeType(ext);
                            aosTarget.PutObject(aosTarget.OssConfig.BucketName, targetKey, fs, metadata);

                            uplistkey.Remove(sourceKey);
                            index++;
                        }
                        catch (Exception ex)
                        {
                            tempuploaderror = true;
                            Console.WriteLine("上传中出错,siteid:{0},  message:{1}", tempsiteid, ex.Message);
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                    if (!tempuploaderror)//上传成功, 删除本地文件
                        File.Delete(sourceKey);
                }
                Console.WriteLine("siteid:{0}上传完成", tempsiteid);
                #endregion

                //该站点完成, 写入日志
                FinishedList.Add(new { SiteID = tempsiteid, FinishedDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });

                Console.WriteLine("siteid:{0}写入日志\n\n", tempsiteid);
                string strtemp = JsonConvert.SerializeObject(FinishedList);
                File.WriteAllText(FinishedIdsFile, strtemp);
            }
            
            Console.WriteLine("\n\n\n\n任务全部完成, 请查看日志");
            Console.ReadLine();
        }


        private static void TempFunction(AliyunOpenStorageService aos)
        {
            Aliyun.OpenServices.OpenStorageService.ObjectListing result = null;
            string nextMarker = string.Empty;
            try
            {
                //do
                //{
                    #region 往list里面写入这个站点所有的文件key

                    var listObjectsRequest = new Aliyun.OpenServices.OpenStorageService.ListObjectsRequest(aos.OssConfig.BucketName)
                    {
                        Marker = nextMarker,
                        MaxKeys = 100
                        ,
                        Prefix = ""
                    };
                    result = aos.ListObjects(listObjectsRequest);


                    foreach (var summary in result.ObjectSummaries)
                    {
                        var tempkey = summary.Key;
                        var tempkeys = tempkey.Split('/');
                        var filename = tempkeys[tempkeys.Length - 1];
                        if (filename.Split('.').Length != 2) //如果没有后缀名. 那就不管了
                            continue;
                        
                    }
                    nextMarker = result.NextMarker;

                    #endregion
                    Console.WriteLine("读取到{0}个文件", result.ObjectSummaries.Count());
                //} while (result.IsTruncated);//(false);// 
            }
            catch (Exception ex)
            {
                Console.WriteLine("error {0}", ex.Message);
            }
        }


        private static void VisitUSA()
        {
            //使用这个config可以读取到文件. 这个config是杭州阿里云
            //var config = new
            //{
            //    EndPoint = "http://oss.aliyuncs.com",
            //    AccessID = "8LCEEFOwxD3cEo0i",
            //    AccessKey = "f4Yexhzq6juh8gWUgWJWBPS3frphv6",
            //    BucketName = "wezhan"
            //};

            //usa
            var config = new
            {
                EndPoint = "http://oss-us-west-1.aliyuncs.com",
                AccessID = "8LCEEFOwxD3cEo0i",
                AccessKey = "f4Yexhzq6juh8gWUgWJWBPS3frphv6",
                BucketName = "wezhanus"
            };
            var _ossClient = new Aliyun.OpenServices.OpenStorageService.OssClient(config.EndPoint, config.AccessID, config.AccessKey);
 
            Aliyun.OpenServices.OpenStorageService.ObjectListing result = null;
            string nextMarker = string.Empty;
            try
            {
                //do
                //{
                #region 往list里面写入这个站点所有的文件key

                var listObjectsRequest = new Aliyun.OpenServices.OpenStorageService.ListObjectsRequest(config.BucketName)
                {
                    Marker = nextMarker,
                    MaxKeys = 100
                    ,
                    Prefix = ""
                };
                result = _ossClient.ListObjects(listObjectsRequest);


                foreach (var summary in result.ObjectSummaries)
                {
                    var tempkey = summary.Key;
                    var tempkeys = tempkey.Split('/');
                    var filename = tempkeys[tempkeys.Length - 1];
                    if (filename.Split('.').Length != 2) //如果没有后缀名. 那就不管了
                        continue;

                }
                nextMarker = result.NextMarker;

                #endregion
                Console.WriteLine("读取到{0}个文件", result.ObjectSummaries.Count());
                //} while (result.IsTruncated);//(false);// 
            }
            catch (Exception ex)
            {
                Console.WriteLine("error {0}", ex.Message);
            }
        }
    }


}
