using FastDev.Log;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace Ueditor.AppCode
{
    public static class QiniutokenHelper
    {
        public static string GetToken()
        {
            string token = HttpContext.Current.Cache.Get("UploadToken")?.ToString();
            if (string.IsNullOrEmpty(token))
            {
                token = GetTokenFromQiniu();
            }
            return token;
        }
        public static string GetTokenFromQiniu()
        {
            string token = "";
            try
            {
                var accessKey = ConfigurationManager.AppSettings["qiniukey"];//填写在你七牛后台找到相应的accesskey "TYb8ZurNoN-xrUOhnNom_q3ZdPl1OLJqUsvoP0xB";
                var secretKey = ConfigurationManager.AppSettings["qiniusercet"]; //填写在你七牛后台找到相应的secretkey""d7ajdjoitbUiJZQY7ANw5bSf3p6K3nQA8KkGxIDq";;
                Mac mac = new Mac(accessKey, secretKey);
                // 空间名
                string Bucket = ConfigurationManager.AppSettings["bucket"];
                // 设置上传策略，详见：https://developer.qiniu.com/kodo/manual/1206/put-policy
                PutPolicy putPolicy = new PutPolicy();
                putPolicy.Scope = Bucket;
                putPolicy.SetExpires(7200);
                token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
                HttpContext.Current.Cache.Add("UploadToken", token, null, DateTime.Now.AddHours(2), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "QiniutokenHelper/GetTokenFromQiniu");

            }
            return token;
        }

        /// <summary>
        /// 上传到七牛
        /// </summary>
        /// <param name="myStream"></param>
        /// <param name="filename"></param>
        /// <param name="servePath">上传成功的路径</param>
        /// <returns></returns>
        public static HttpResult UploadFile(Stream myStream,string filename,out string servePath)
        {
            //code为200成功
            HttpResult result = new HttpResult() { Code = 1 };
            servePath = "";
            try
            {
                // 上传文件名
                servePath = $"{Guid.NewGuid().ToString().Replace("-", "")}{filename}";
                Qiniu.Storage.Config config = new Qiniu.Storage.Config();
                // 设置上传区域
                config.Zone = Zone.ZONE_CN_South;
                // 设置 http 或者 https 上传
                config.UseHttps = true;
                config.UseCdnDomains = true;
                config.ChunkSize = ChunkUnit.U512K;
                ResumableUploader target = new ResumableUploader(config);
                PutExtra extra = new PutExtra();
                string token = GetToken();
                result = target.UploadStream(myStream, servePath, token, extra);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "QiniutokenHelper/UploadFile");
            }
            return result;
        }
    }
}