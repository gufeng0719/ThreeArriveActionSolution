using System;
using System.IO;
using System.Net;
using System.Text;

namespace ThreeArriveAction.Common
{
    public class HttpHelper
    {
        /// <summary>
        /// 后台发送POST请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public static string HttpPost(string url, string data)
        {
            try
            {
                //创建post请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                byte[] payload = Encoding.UTF8.GetBytes(data);
                request.ContentLength = payload.Length;

                //发送post的请求
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();

                //接受返回来的数据
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string value = reader.ReadToEnd();

                reader.Close();
                stream.Close();
                response.Close();

                return value;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 后台发送GET请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            try
            {
                //创建Get请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                //接受返回来的数据
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                string retString = streamReader.ReadToEnd();

                streamReader.Close();
                stream.Close();
                response.Close();

                return retString;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 上传临时素材
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public static string UploadMultimedia(string filePath, string accessToken, MediaType type)
        {
            string result;
            var wxurl = $"https://api.weixin.qq.com/cgi-bin/media/upload?access_token={accessToken}&type={type}";
            var myWebClient = new WebClient { Credentials = CredentialCache.DefaultCredentials };
            try
            {
                var responseArray = myWebClient.UploadFile(wxurl, "POST", filePath);
                result = Encoding.Default.GetString(responseArray, 0, responseArray.Length);
            }
            catch (Exception ex)
            {
                result = "Error:" + ex.Message;
            }
            return result;
        }
    }
}
