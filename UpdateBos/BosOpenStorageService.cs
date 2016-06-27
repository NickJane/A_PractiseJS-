using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos;
using System.IO;
using BaiduBce.Services.Bos.Model;

namespace UpdateBos
{
    class BosOpenStorageService
    {
        private BceClientConfiguration _clientConfiguration;
        private OssConfig _ossConfig;
        private BosClient _bosClient;
        public BosOpenStorageService(OssConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config can not be null!");
            }
            _ossConfig = config;
            _clientConfiguration = new BceClientConfiguration();
            _clientConfiguration.Credentials = new DefaultBceCredentials(config.AccessID, config.AccessKey);
            _clientConfiguration.Endpoint = config.EndPoint;
            _bosClient = new BosClient(_clientConfiguration);
        }
        public OssConfig OssConfig
        {
            get { return _ossConfig; }
        }

        public PutObjectResponse PutObject(string bucketName, string key, Stream content, ObjectMetadata metadata)
        {
            BaiduBce.Services.Bos.Model.ObjectMetadata bMetaData = new BaiduBce.Services.Bos.Model.ObjectMetadata();
            bMetaData.ContentLength = metadata.ContentLength;
            bMetaData.ContentType = metadata.ContentType;
            BaiduBce.Services.Bos.Model.PutObjectResponse response = _bosClient.PutObject(bucketName, key, content, bMetaData);

            return response;
        }

        
        public List<BosObjectSummary> ListObjects(string bucketName, string prefix)
        {
            BaiduBce.Services.Bos.Model.ListObjectsResponse response = _bosClient.ListObjects(bucketName, prefix);
            //response.
            //ObjectListing list = BuilderObjectLising(bucketName, response);
            return response.Contents;
        }

        public void deleteObject(string bucketName, string key) {
            _bosClient.DeleteObject(bucketName, key);
        }
    }
}
