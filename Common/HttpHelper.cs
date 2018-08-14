﻿using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Common
{
    public static class HttpHelper
    {
        /// <summary>  
        /// POST请求与获取结果  
        /// </summary>  
        public static string HttpPost(string url, string postDataStr)
        {
            try
            {
                url += "?macAddress=" + GetMacAddress();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postDataStr.Length;
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(postDataStr);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码  
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                return retString;
            }
            catch (Exception e)
            {
                return "";
            }
           
        }

        /// <summary>
        /// 模拟HTTP提交表单并获取返回数据
        /// GET
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="postDataStr">参数</param>
        /// <returns></returns>
        public static string HttpGet(string url, string postDataStr)
        {
            try
            {
                postDataStr += "&macAddress=" + GetMacAddress();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (Exception e)
            {
                return "";
            }            
        }

        public static string GetMacAddress()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up && !string.IsNullOrEmpty(ni.GetPhysicalAddress().ToString()))
                {
                    return ni.GetPhysicalAddress().ToString();
                }
            }

            return "";
        }
    }
}
