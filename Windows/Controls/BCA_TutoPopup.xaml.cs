using hub_client.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_TutoPopup.xaml
    /// </summary>
    public partial class BCA_TutoPopup : UserControl
    {
        public event Action<BCA_TutoPopup> NextStep;
        public event Action<BCA_TutoPopup> SkipTuto;

        public BCA_TutoPopup()
        {
            InitializeComponent();
            LoadStyle();

            btnNext.MouseLeftButtonDown += NextStepClick;
            btnSkip.MouseLeftButtonDown += SkipClick;
        }

        private void SkipClick(object sender, MouseButtonEventArgs e)
        {
            SkipTuto?.Invoke(this);
        }

        private void NextStepClick(object sender, MouseButtonEventArgs e)
        {
            NextStep?.Invoke(this);
        }

        private void LoadStyle()
        {
            AppDesignConfig style = FormExecution.AppDesignConfig;
            this.FontFamily = style.Font;

            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btnNext, btnSkip });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1PrestigeShopButton");
                btn.Color2 = style.GetGameColor("Color2PrestigeShopButton");
                btn.Update();
            }
        }

        public void SetText(string txt)
        {
            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            popText.RenderTransformOrigin = new Point(0.5, 0.5);
            popText.RenderTransform = scale;

            DoubleAnimation growAnimationClose = new DoubleAnimation();
            growAnimationClose.Duration = TimeSpan.FromMilliseconds(100);
            growAnimationClose.From = 1.0;
            growAnimationClose.To = 0.0;
            storyboard.Children.Add(growAnimationClose);

            Storyboard.SetTargetProperty(growAnimationClose, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationClose, popText);

            storyboard.Completed += (sender, e) => changeTextAnimation_Completed(sender, e, txt);
            storyboard.Begin();
        }

        private void changeTextAnimation_Completed(object sender, EventArgs e, string txt)
        {
            popText.Text = txt;
            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            popText.RenderTransformOrigin = new Point(0.5, 0.5);
            popText.RenderTransform = scale;

            DoubleAnimation growAnimationOpen = new DoubleAnimation();
            growAnimationOpen.Duration = TimeSpan.FromMilliseconds(100);
            growAnimationOpen.From = 0.0;
            growAnimationOpen.To = 1.0;
            storyboard.Children.Add(growAnimationOpen);
            Storyboard.SetTargetProperty(growAnimationOpen, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationOpen, popText);

            storyboard.Begin();
        }
    }
}
