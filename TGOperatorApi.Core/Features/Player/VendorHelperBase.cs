using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TgOperator.Core.Features.Player.Model;

namespace TgOperator.Core.Features.Player
{
    public abstract class VendorHelperBase 
    {
        public static int proxyPort;
        public static string proxyHost;
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; }



        public static string HttpPost(string url, string parameters, string contentType, Dictionary<string, string> headers = null, int timeOutTime = 0)
        {
            try
            {
                var parametersLog = parameters;
                if (headers != null)
                {
                    parametersLog = $"{parametersLog}, Header={JsonConvert.SerializeObject(headers)}";
                }

                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                }

                WebRequest request = WebRequest.Create(url);
                if (!string.IsNullOrEmpty(proxyHost))
                {
                    request.Proxy = new WebProxy(proxyHost, proxyPort);
                }

                if (timeOutTime > 0)
                {
                    request.Timeout = timeOutTime * 1000;
                }

                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
                request.ContentType = contentType;
                request.ContentLength = byteArray.Length;

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        request.Headers.Add(item.Key, item.Value);
                    }
                }
                using (Stream RequestStream = request.GetRequestStream())
                {
                    RequestStream.Write(byteArray, 0, byteArray.Length);
                }

                StringBuilder sb = new StringBuilder();
                using (WebResponse response = request.GetResponse())
                using (Stream ResponseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(ResponseStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureMessage(e.Message, SentryLevel.Error);
                return e.Message;
            }
        }

        public static string Encrypt(string req, VGConfig config)
        {
            var s = req + config.OperatorKey;
            var sign = ToMD5(s);
            return sign;
        }

        public static string ToMD5(string encodeStr)
        {
            using var cryptoMD5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(encodeStr);
            var hash = cryptoMD5.ComputeHash(bytes);
            var md5 = BitConverter.ToString(hash)
              .Replace("-", string.Empty);

            return md5;
        }

        public static string HashHMACHex(string keyHex, string messageHex)
        {
            HMACSHA256 hashObject = new HMACSHA256(Encoding.UTF8.GetBytes(keyHex));

            var signature = hashObject.ComputeHash(Encoding.UTF8.GetBytes(messageHex));

            return Convert.ToBase64String(signature);
        }
    }
}
