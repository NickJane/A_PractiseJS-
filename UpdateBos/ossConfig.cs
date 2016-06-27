
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UpdateBos.helper;

namespace UpdateBos
{
    /// <summary>
    /// OSS的配置信息
    /// </summary>
    public class OssConfig
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 图片处理服务器域名
        /// </summary>
        public string ImageDomain { get; set; }

        /// <summary>
        /// 访问ID
        /// </summary>
        public string AccessID { get; set; }

        /// <summary>
        /// 访问KEY
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// 是否本地OSS
        /// </summary>
        public bool IsLocalOss { get; set; }

        /// <summary>
        /// 设计状态是否启用该OSS
        /// </summary>
        public bool ValidInDesign { get; set; }

        /// <summary>
        /// 发布到虚机后是否启用该OSS
        /// </summary>
        public bool ValidInPublished { get; set; }

        /// <summary>
        /// 当前OSS支持哪几类SKU
        /// </summary>
        public string[] SupportSkuIds { get; set; }

        /// <summary>
        /// Oss的接入点
        /// </summary>
        public string EndPoint { get; set; }

        /// <summary>
        /// 文件根目录
        /// </summary>
        public string RootFilePath { get; set; }
        /// <summary>
        /// Oss上的Bucket单元名称
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// 平台名称
        /// </summary>
        public string Platform { get; set; }
        /// <summary>
        /// 根据XML的节点实例化一个Oss Config对象
        /// </summary>
        /// <param name="node">XML节点</param>
        /// <returns></returns>
        internal static OssConfig LoadByXmlNode(XmlNode node)
        {
            OssConfig config = new OssConfig();
            config.Domain = Utilities.SafeGetAttribute(node, "domain");
            config.ImageDomain = Utilities.SafeGetAttribute(node, "imgdomain");
            config.AccessID = Utilities.SafeGetAttribute(node, "accessId");
            config.AccessKey = Utilities.SafeGetAttribute(node, "accessKey");
            config.IsLocalOss = Utilities.SafeGetAttribute(node, "isLocalOss") == "true";
            config.RootFilePath = Utilities.SafeGetAttribute(node, "rootFilePath");
            config.BucketName = Utilities.SafeGetAttribute(node, "bucketName");
            string skuids = Utilities.SafeGetAttribute(node, "skuIds");
            config.SupportSkuIds = skuids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            config.EndPoint = Utilities.SafeGetAttribute(node, "endPoint");
            config.ValidInDesign = Utilities.SafeGetAttribute(node, "validInDesign") != "false";
            config.ValidInPublished = Utilities.SafeGetAttribute(node, "validInPublished") == "true";
            config.Platform = Utilities.SafeGetAttribute(node, "platform");
            return config;
        }

    }

    /// <summary>
    /// Oss配置管理信息
    /// </summary>
    public class OssConfigMgr
    {
        static string fileName = Environment.CurrentDirectory + "\\ossConfig.xml"; 
        public static OssConfig GetOneConfig(string domain)
        {
            return GetAllConfigs().Where(item => item.Domain == domain).FirstOrDefault();
        }

        public static OssConfig GetOneConfig()
        {
            OssConfig config;
            config = OssConfigMgr.GetAllConfigs().FirstOrDefault();
            return config;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">img.andni.cn/img.bwezhan.cn</param>
        /// <returns></returns>
        public static OssConfig GetConfig(int idx) {
            OssConfig config;
            config = OssConfigMgr.GetAllConfigs().Skip(idx - 1).First();
            return config;
        }
        public static IList<OssConfig> GetAllConfigs()
        {
             IList<OssConfig> list;
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);

                XmlNodeList nodes = doc.DocumentElement.SelectNodes("instant");
                list = new List<OssConfig>();
                foreach (XmlNode node in nodes)
                {
                    OssConfig config = OssConfig.LoadByXmlNode(node);
                    list.Add(config);
                }

            
            return list;
        }
    }
}
