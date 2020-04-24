using hub_client.Configuration;
using System;
using System.Windows;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour DuelResult.xaml
    /// </summary>
    public partial class DuelResult : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        public DuelResult(int bp, int exp, bool win)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            string text;
            if (win)
                text = "Félicitations pour ta victoire !";
            else
                text = "Dommage tu viens de perdre... Tu feras mieux la prochaine fois !";
            text += Environment.NewLine + Environment.NewLine;

            text += "Tu as remporté " + bp.ToString() + " BPs et " + exp.ToString() + " points d'expériences.";

            popText.Text = text;

            this.Loaded += DuelResult_Loaded;
        }

        private void DuelResult_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
        }

        private void btnAgree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        private void LoadStyle()
        {
            btnAgree.Color1 = style.GetGameColor("Color1DuelRequestButton");
            btnAgree.Color2 = style.GetGameColor("Color2DuelRequestButton");
            btnAgree.Update();

            this.FontFamily = style.Font;
        }
    }
}
