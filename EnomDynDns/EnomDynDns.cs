using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;

namespace EnomDynDns
{
    public partial class EnomDynDns : ServiceBase
    {
        private readonly System.Timers.Timer _timer;

        public EnomDynDns()
        {
            InitializeComponent();
            if (!EventLog.SourceExists("EnomDynDns"))
            {
                EventLog.CreateEventSource(
                    "EnomDynDns", "EnomDynDnsLog");
            }
            eventLog1.Source = "EnomDynDns";
            eventLog1.Log = "EnomDynDnsLog";
            
            long timel;
            _timer = new System.Timers.Timer(long.TryParse(GetAppSetting("UpdateFrequency"), out timel) ? timel : 3600000);
            _timer.Elapsed += timer_Elapsed;

            CanPauseAndContinue = true;
        }

        protected override async void OnStart(string[] args)
        {
            eventLog1.WriteEntry("EnomDynDns Starting...");
            Dns.Init();
			var res = await Dns.Update();
            if (!res.IsSuccessStatusCode) {
                eventLog1.WriteEntry($"Initial update failed: {res.Content}", EventLogEntryType.Error);
                ExitCode = 1;
                Stop();
            }
            _timer.Enabled = true;
            eventLog1.WriteEntry("Initial update succeeded...");
            eventLog1.WriteEntry("EnomDynDns Started...");
        }

        private async void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            try {
				var resp = await Dns.Update();
				eventLog1.WriteEntry($"Server update {(resp.IsSuccessStatusCode ? "succeeded" : $"failed: {resp.Content}")}");
            }
            catch (Exception exception) {
                eventLog1.WriteEntry(exception.Message, EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("EnomDynDns Stopping...");
            _timer.Enabled = false;
            eventLog1.WriteEntry("EnomDynDns Stopped...");
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("EnomDynDns Continuing...");
            base.OnContinue();
            _timer.Enabled = false;
            eventLog1.WriteEntry("EnomDynDns Continued...");
        }

        protected override void OnPause()
        {
            eventLog1.WriteEntry("EnomDynDns Pausing...");
            base.OnPause();
            _timer.Enabled = false;
            eventLog1.WriteEntry("EnomDynDns Paused...");
        }

        private static string GetAppSetting(string key) {
            try {
                var setting = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrEmpty(setting))
                    throw new ArgumentNullException($"{key} AppSetting may not be missing, null, nor empty.");
                return setting;
            }
            catch (Exception e) {
                return null;
            }
        }
    }
}
