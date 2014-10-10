using System;
using System.Configuration;
using System.Net.Http;

namespace EnomDynDns
{
    public static class Dns
    {
        public static void Update()
        {
            var host = GetAppSetting("host");
            var zone = GetAppSetting("zone");
            var password = GetAppSetting("domainPassword");
            var enomServer = GetAppSetting("enomServer");

            var httpClient = new HttpClient();
            httpClient.GetAsync(new Uri(string.Format(enomServer, host, zone, password))).ContinueWith(
                (requestTask) =>
                {
                    var response = requestTask.Result;
                    response.EnsureSuccessStatusCode();
                });
        }

        private static string GetAppSetting(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            if(string.IsNullOrEmpty(setting))
                throw new ArgumentNullException(string.Format("{0} AppSetting may not be missing, null, nor empty.", key));
            return setting;
        }
    }
}
