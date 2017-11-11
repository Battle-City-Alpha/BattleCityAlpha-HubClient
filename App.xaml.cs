using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            ShutdownMode = ShutdownMode.OnLastWindowClose;
            Main BCA = new hub_client.Main();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            logger.Trace("[APPLICATION] Exit : sender -> {0}, reason -> {1}", sender.ToString(), e.ToString());
        }
    }
}
