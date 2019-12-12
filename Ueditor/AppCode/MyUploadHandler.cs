using FastDev.Log;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Ueditor.AppCode;

namespace Ueditor
{


    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    public class MyUploadHandler : Handler
    {

        public UploadConfig UploadConfig { get; private set; }
        public UploadResult Result { get; private set; }

        public MyUploadHandler(HttpContext context, UploadConfig config)
            : base(context)
        {
            this.UploadConfig = config;
            this.Result = new UploadResult() { State = UploadState.Unknown };
        }

        public override void Process()
        {
            try
            {
                string uploadFileName = null;
                var accessKey = ConfigurationManager.AppSettings["qiniukey"];//填写在你七牛后台找到相应的accesskey "TYb8ZurNoN-xrUOhnNom_q3ZdPl1OLJqUsvoP0xB";
                var secretKey = ConfigurationManager.AppSettings["qiniusercet"]; //填写在你七牛后台找到相应的secretkey""d7ajdjoitbUiJZQY7ANw5bSf3p6K3nQA8KkGxIDq";;
                HttpPostedFile file = Request.Files[UploadConfig.UploadFieldName];
                Stream myStream = file.InputStream;

                uploadFileName = file.FileName;

                if (!CheckFileType(uploadFileName))
                {
                    Result.State = UploadState.TypeNotAllow;
                    WriteResult();
                    return;
                }
                if (!CheckFileSize(file.ContentLength))
                {
                    Result.State = UploadState.SizeLimitExceed;
                    WriteResult();
                    return;
                }

                HttpResult result = QiniutokenHelper.UploadFile(myStream,uploadFileName,out string key);
                if (result.Code == 200)
                {
                    this.Result.State = UploadState.Success;
                    // this.Result.Url = $"http://ptdzsd6xm.bkt.clouddn.com{Path.DirectorySeparatorChar}{key}";
                    this.Result.Url = key;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "Process");
                throw ex;
            }

            finally
            {
                WriteResult();
            }


        }
        //public override void Process()
        //{
        //    try
        //    {
        //        string uploadFileName = null;
        //        var accessKey = ConfigurationManager.AppSettings["qiniukey"];//填写在你七牛后台找到相应的accesskey "TYb8ZurNoN-xrUOhnNom_q3ZdPl1OLJqUsvoP0xB";
        //        var secretKey = ConfigurationManager.AppSettings["qiniusercet"]; //填写在你七牛后台找到相应的secretkey""d7ajdjoitbUiJZQY7ANw5bSf3p6K3nQA8KkGxIDq";;
        //        HttpPostedFile file = Request.Files[UploadConfig.UploadFieldName];
        //        Stream myStream = file.InputStream;

        //        uploadFileName = file.FileName;

        //        if (!CheckFileType(uploadFileName))
        //        {
        //            Result.State = UploadState.TypeNotAllow;
        //            WriteResult();
        //            return;
        //        }
        //        if (!CheckFileSize(file.ContentLength))
        //        {
        //            Result.State = UploadState.SizeLimitExceed;
        //            WriteResult();
        //            return;
        //        }
        //        Mac mac = new Mac(accessKey, secretKey);
        //        // 上传文件名
        //        string key = $"{Guid.NewGuid().ToString().Replace("-", "")}{uploadFileName}";
        //        // 本地文件路径
        //        //string filePath = @"E:\MY\FastDevStu\Mvc\Models\微信图片_20190508193805.gif";
        //        // 空间名
        //        string Bucket = ConfigurationManager.AppSettings["bucket"];
        //        // 设置上传策略，详见：https://developer.qiniu.com/kodo/manual/1206/put-policy
        //        //PutPolicy putPolicy = new PutPolicy();
        //        //putPolicy.Scope = Bucket + ":" + key;
        //        //putPolicy.SetExpires(7200);
        //        //string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        //        string token = QiniutokenHelper.GetToken();
        //        Qiniu.Storage.Config config = new Qiniu.Storage.Config();
        //        // 设置上传区域
        //        config.Zone = Zone.ZONE_CN_South;
        //        // 设置 http 或者 https 上传
        //        config.UseHttps = true;
        //        config.UseCdnDomains = true;
        //        config.ChunkSize = ChunkUnit.U4096K;
        //        ResumableUploader target = new ResumableUploader(config);
        //        PutExtra extra = new PutExtra();
        //        //设置断点续传进度记录文件,,报文件没又访问权限
        //        //extra.ResumeRecordFile = ResumeHelper.GetDefaultRecordKey(uploadFileName, key);
        //        //Console.WriteLine("record file:" + extra.ResumeRecordFile);
        //        //extra.ResumeRecordFile = "test.progress";
        //        HttpResult result = target.UploadStream(myStream, key, token, extra);
        //        if (result.Code == 200)
        //        {
        //            this.Result.State = UploadState.Success;
        //            // this.Result.Url = $"http://ptdzsd6xm.bkt.clouddn.com{Path.DirectorySeparatorChar}{key}";
        //            this.Result.Url = key;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        LogHelper.WriteLog(ex, "Process");
        //        throw ex;
        //    }

        //    finally
        //    {
        //        WriteResult();
        //    }


        //}

        private void WriteResult()
        {
            this.WriteJson(new
            {
                state = GetStateMessage(Result.State),
                url = Result.Url,
                title = Result.OriginFileName,
                original = Result.OriginFileName,
                error = Result.ErrorMessage
            });
        }

        private string GetStateMessage(UploadState state)
        {
            switch (state)
            {
                case UploadState.Success:
                    return "SUCCESS";
                case UploadState.FileAccessError:
                    return "文件访问出错，请检查写入权限";
                case UploadState.SizeLimitExceed:
                    return "文件大小超出服务器限制";
                case UploadState.TypeNotAllow:
                    return "不允许的文件格式";
                case UploadState.NetworkError:
                    return "网络错误";
            }
            return "未知错误";
        }

        private bool CheckFileType(string filename)
        {
            var fileExtension = Path.GetExtension(filename).ToLower();
            return UploadConfig.AllowExtensions.Select(x => x.ToLower()).Contains(fileExtension);
        }

        private bool CheckFileSize(int size)
        {
            return size < UploadConfig.SizeLimit;
        }
    }
}

//public class UploadConfig
//{
//    /// <summary>
//    /// 文件命名规则
//    /// </summary>
//    public string PathFormat { get; set; }

//    /// <summary>
//    /// 上传表单域名称
//    /// </summary>
//    public string UploadFieldName { get; set; }

//    /// <summary>
//    /// 上传大小限制
//    /// </summary>
//    public int SizeLimit { get; set; }

//    /// <summary>
//    /// 上传允许的文件格式
//    /// </summary>
//    public string[] AllowExtensions { get; set; }

//    /// <summary>
//    /// 文件是否以 Base64 的形式上传
//    /// </summary>
//    public bool Base64 { get; set; }

//    /// <summary>
//    /// Base64 字符串所表示的文件名
//    /// </summary>
//    public string Base64Filename { get; set; }
//}

//public class UploadResult
//{
//    public UploadState State { get; set; }
//    public string Url { get; set; }
//    public string OriginFileName { get; set; }

//    public string ErrorMessage { get; set; }
//}

//public enum UploadState
//{
//    Success = 0,
//    SizeLimitExceed = -1,
//    TypeNotAllow = -2,
//    FileAccessError = -3,
//    NetworkError = -4,
//    Unknown = 1,
//}

