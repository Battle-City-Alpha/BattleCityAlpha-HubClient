using NLog;
using System;
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
            try
            {
                Main BCA = new hub_client.Main();
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                FormExecution.Client_PopMessageBox("Une erreur s'est produite." + ex.ToString(), "Problème");
                Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            logger.Trace("[APPLICATION] Exit : sender -> {0}, reason -> {1}", sender.ToString(), e.ToString());
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Error("UNHANDLED EXCEPTION - {0}", e.Exception.ToString());
            FormExecution.Client_PopMessageBoxShowDialog("Une erreur s'est produite.", "Problème");
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
    }
}
