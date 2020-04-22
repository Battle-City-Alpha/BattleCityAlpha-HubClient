using System.Windows.Controls;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_TextBox.xaml
    /// </summary>
    public partial class BCA_TextBox : UserControl
    {
        public BCA_TextBox()
        {
            InitializeComponent();
        }

        public string GetText()
        {
            return tbChat.Text;
        }
        public void SetText(string txt)
        {
            tbChat.Text = txt;
        }

        public void Clear()
        {
            tbChat.Clear();
        }
        public void RefreshStyle()
        {
            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }
    }
}
