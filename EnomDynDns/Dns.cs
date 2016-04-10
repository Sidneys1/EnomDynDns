using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace EnomDynDns
{
    public static class Dns
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static Uri _uri;

        public static void Init() {
            _uri = new Uri(string.Format(
                GetAppSetting("enomServer"), 
                GetAppSetting("host"), 
                GetAppSetting("zone"), 
                GetAppSetting("domainPassword")
            ));
        }

        public static async Task<HttpResponseMessage> Update() 
            => await HttpClient.GetAsync(_uri);

        private static string GetAppSetting(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            if(string.IsNullOrEmpty(setting))
                throw new ArgumentNullException($"{key} AppSetting may not be missing, null, nor empty.");
            return setting;
        }
    }
}
