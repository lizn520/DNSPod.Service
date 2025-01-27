﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DNSPod.Service
{
    public class Dnsapi
    {
        private const string ApiUrl = "https://dnsapi.cn/";

        public const string List = ApiUrl + "Record.List";

        public const string Create = ApiUrl + "Record.Create";

        public const string Ddns = ApiUrl + "Record.Ddns";

        public const string Modify = ApiUrl + "Record.Modify";
    }

    public class Helpter
    {
        public static string GetPublicIP()
        {
            return GetHtml("https://4.ipw.cn/");
        }

        public static string GetPublicIPV6()
        {
            return GetHtml("https://6.ipw.cn/");
        }

        public static dynamic RecordList(string token, string domain, string sub_domain, string record_type = "A")
        {
            return Post(Dnsapi.List, $"{CommonParam(token)}&domain={domain}&sub_domain={sub_domain}&record_type={record_type}");
        }

        public static dynamic RecordCreate(string token, string domain, string sub_domain, string value, string record_type = "A", string record_line_id = "0")
        {
            dynamic result = Post(Dnsapi.Create, $"{CommonParam(token)}&domain={domain}&sub_domain={sub_domain}&value={value}&record_type={record_type}&record_line_id={record_line_id}");
            return Convert.ToInt32(result.status.code) == 1 ? Convert.ToInt32(result.record.id) : -1;
        }

        public static dynamic RecordDdns(string token, int record_id, string domain, string sub_domain, string value, string record_line = "默认")
        {
            return Post(Dnsapi.Ddns,
                $"{CommonParam(token)}&record_id={record_id}&domain={domain}&sub_domain={sub_domain}&value={value}&record_line={record_line}");
        }

        public static dynamic RecordModify(string token, int record_id, string domain, string sub_domain, string value, int ttl = 100, string record_line = "默认", string record_type = "A", string record_line_id = "0")
        {
            return Post(Dnsapi.Modify,
                $"{CommonParam(token)}&record_id={record_id}&domain={domain}&sub_domain={sub_domain}&value={value}&record_line={record_line}&record_type={record_type}&record_line_id={record_line_id}");
        }

        private static string CommonParam(string token)
        {
            return $"login_token={token}&format=json&lang=cn&error_on_empty=no";
        }

        private static dynamic Post(string url, string data)
        {
            using (var client = new HttpClient())
            {
                HttpContent content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Add("User-Agent", "DNSPod.Service/V1.0 (uhnnamed@qq.com)");
                var result = client.PostAsync(url, content).Result;
                if (result.IsSuccessStatusCode)
                {
                    string json = result.Content.ReadAsStringAsync().Result;
                    return JObject.Parse(json);
                }
            }
            return string.Empty;
        }


        public static void WriteLineLog(string path, string category, string log)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine(($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} [{category}] {log}"));
            }
            else
                using (StreamWriter sw = new StreamWriter($"{path}{DateTime.Now.ToString("yyyy-MM-dd")}.txt", true))
                {
                    sw.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} [{category}] {log}");
                }
        }

        /// <summary>
        /// 传入URL返回网页的html代码
        /// </summary>
        /// <param name="Url">URL</param>
        /// <returns></returns>
        private static string GetHtml(string url)
        {
            try
            {
                HttpWebRequest request;
                HttpWebResponse response;
                request = WebRequest.Create(url) as HttpWebRequest;
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request.ProtocolVersion = HttpVersion.Version11;
                }
                request.Method = "GET";
                request.ContentType = "text/html";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36";
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8))
                            return reader.ReadToEnd();
                    }
                }
            }
            catch (System.Exception ex)
            {
            }
            return string.Empty;
        }
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
    }

}
