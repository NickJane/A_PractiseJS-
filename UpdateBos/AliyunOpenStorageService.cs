using Aliyun.OpenServices.OpenStorageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBos
{
    class AliyunOpenStorageService
    {
        private OssClient _ossClient;
        private OssConfig _ossConfig;
        public OssConfig OssConfig
        {
            get { return _ossConfig; }
        }
        /// <summary>
        /// 通过一个OSS配置初始化阿里云OSS
        /// </summary>
        /// <param name="config"></param>
        public AliyunOpenStorageService(OssConfig config)
        {
            _ossClient = new OssClient(config.EndPoint, config.AccessID, config.AccessKey);
            _ossConfig = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="prefix">key+"/"</param>
        /// <returns></returns>
        public ObjectListing ListObjects(string bucketName, string prefix)
        {
            return _ossClient.ListObjects(bucketName, prefix);
        }
        public ObjectListing ListObjects(ListObjectsRequest request)
        {
            return _ossClient.ListObjects(request);
        }
        public OssObject GetObject(string bucketName, string key)
        {
            return _ossClient.GetObject(bucketName, key);
        }
        public PutObjectResult PutObject(string bucketName, string key, System.IO.Stream content, 
            Aliyun.OpenServices.OpenStorageService.ObjectMetadata metadata)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException("bucketName can not be null or empty!");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key can not be null or empty!");
            }
            if (content == null)
            {
                throw new ArgumentNullException("content can not be null!");
            }
            return _ossClient.PutObject(bucketName, key, content, metadata);
        }
    }
}
