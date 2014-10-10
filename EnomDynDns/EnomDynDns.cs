using System.ServiceProcess;

namespace EnomDynDns
{
    public partial class EnomDynDns : ServiceBase
    {
        private System.Timers.Timer _timer;

        public EnomDynDns()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("EnomDynDns"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "EnomDynDns", "EnomDynDnsLog");
            }
            eventLog1.Source = "EnomDynDns";
            eventLog1.Log = "EnomDynDnsLog";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("EnomDynDns Starting...");

            Dns.Update();

            _timer = new System.Timers.Timer(300);
            _timer.Elapsed += timer_Elapsed;
            _timer.Enabled = true;

            eventLog1.WriteEntry("EnomDynDns Started...");
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dns.Update();
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
            base.OnContinue();
            _timer.Enabled = true;
            eventLog1.WriteEntry("EnomDynDns Paused...");
        }
    }
}
