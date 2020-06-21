using hub_client.Configuration;
using System;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour InputText.xaml
    /// </summary>
    public partial class InputText : Window
    {
        public event Action<string> SelectedText;
        private bool _firstTime = true;
        private string _ph = "";

        public InputText(string placeholder)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            LoadStyle();

            this.tb_text.tbChat.VerticalContentAlignment = VerticalAlignment.Center;

            _ph = placeholder;
            this.tb_text.tbChat.Text = _ph;
            this.tb_text.tbChat.SelectionChanged += DeletePlaceholder;

            this.MouseDown += Window_MouseDown;
        }

        private void DeletePlaceholder(object sender, RoutedEventArgs e)
        {
            if (_firstTime)
            {
                _firstTime = false;
                this.tb_text.tbChat.Text = "";
            }
        }

        private void btnAgree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (tb_text.GetText() == "")
                return;

            SelectedText?.Invoke(tb_text.GetText());
            Close();
        }

        public void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;

            btnAgree.Color1 = style.GetGameColor("Color1PopBoxButton");
            btnAgree.Color2 = style.GetGameColor("Color2PopBoxButton");
            btnAgree.Update();

            this.FontFamily = style.Font;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void minimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch { }
        }
    }
}
