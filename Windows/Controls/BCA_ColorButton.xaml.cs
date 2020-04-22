using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace hub_client.Windows.Controls
{
    /// <summary>
    /// Logique d'interaction pour BCA_ColorButton.xaml
    /// </summary>
    public partial class BCA_ColorButton : UserControl
    {
        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register
              (
                   "ButtonText",
                   typeof(string),
                   typeof(BCA_ColorButton),
                   new PropertyMetadata(string.Empty)
              );

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextSizeProperty = DependencyProperty.Register
              (
                   "ButtonTextSize",
                   typeof(int),
                   typeof(BCA_ColorButton),
                   new PropertyMetadata(22)
              );

        public int ButtonTextSize
        {
            get { return (int)GetValue(ButtonTextSizeProperty); }
            set { SetValue(ButtonTextSizeProperty, value); }
        }

        public static readonly DependencyProperty Color1Property = DependencyProperty.Register
              (
                   "Color1",
                   typeof(Color),
                   typeof(BCA_ColorButton),
                   new PropertyMetadata((Color)ColorConverter.ConvertFromString("#FF26164F"))
              );

        public Color Color1
        {
            get { return (Color)GetValue(Color1Property); }
            set { SetValue(Color1Property, value); }
        }

        public static readonly DependencyProperty Color2Property = DependencyProperty.Register
             (
                  "Color2",
                  typeof(Color),
                  typeof(BCA_ColorButton),
                  new PropertyMetadata((Color)ColorConverter.ConvertFromString("#FF221A29"))
             );

        public Color Color2
        {
            get { return (Color)GetValue(Color2Property); }
            set { SetValue(Color2Property, value); }
        }

        public BCA_ColorButton()
        {
            InitializeComponent();
            this.Loaded += BCA_Button_Loaded;
        }

        public void ClickedAnimation()
        {
            border.BorderThickness = new Thickness(3);
        }
        public void ReleasedAnimation()
        {
            border.BorderThickness = new Thickness(2);
        }

        public void Update()
        {
            text.Content = ButtonText;
            text.FontSize = ButtonTextSize;
            GradientStop1.Color = Color1;
            GradientStop2.Color = Color2;
        }

        private void BCA_Button_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClickedAnimation();
        }

        private void border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleasedAnimation();
        }

        public void RefreshStyl()
        {
            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }
    }
}
