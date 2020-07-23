using BCA.Common.Enums;
using hub_client.Configuration;
using hub_client.Helpers;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour DailyQuestWindow.xaml
    /// </summary>
    public partial class DailyQuestWindow : Window
    {
        private DailyQuestAdministrator _admin;

        private string[] _quests;
        public DailyQuestWindow(DailyQuestAdministrator admin, DailyQuestType[] dqtype, string[] quests, int[] states)
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;
            _admin.ChangeQuestState += _admin_ChangeQuestState;
            _admin.ChangeQuest += (t, txt) => _admin_ChangeQuest(t, txt, false);

            this.MouseDown += Window_MouseDown;

            _quests = quests;
            this.tbNormalQuest.Text = quests[0];
            this.tbSpecialQuest.Text = quests[1];
            this.tbFunQuest.Text = quests[2];

            btn_changenormal.Visibility = states[0] <= 0 ? Visibility.Hidden : Visibility.Visible;
            btn_changespecial.Visibility = states[1] <= 0 ? Visibility.Hidden : Visibility.Visible;
            btn_changefun.Visibility = states[2] <= 0 ? Visibility.Hidden : Visibility.Visible;

            btn_getnormal.Visibility = states[0] == 0 ? Visibility.Visible : Visibility.Hidden;
            btn_getspecial.Visibility = states[1] == 0 ? Visibility.Visible : Visibility.Hidden;
            btn_getfun.Visibility = states[2] == 0 ? Visibility.Visible : Visibility.Hidden;

            SetQuestState(states[0], border_normal, tbNormalQuest);
            SetQuestState(states[1], border_special, tbSpecialQuest);
            SetQuestState(states[2], border_fun, tbFunQuest);

            btn_getnormal.MouseLeftButtonDown += (sender, e) => btn_getClicked(sender, e, DailyQuestType.Normal);
            btn_getspecial.MouseLeftButtonDown += (sender, e) => btn_getClicked(sender, e, DailyQuestType.Special);
            btn_getfun.MouseLeftButtonDown += (sender, e) => btn_getClicked(sender, e, DailyQuestType.Fun);

            btn_changenormal.MouseLeftButtonDown += (sender, e) => btn_changeClicked(sender, e, DailyQuestType.Normal);
            btn_changespecial.MouseLeftButtonDown += (sender, e) => btn_changeClicked(sender, e, DailyQuestType.Special);
            btn_changefun.MouseLeftButtonDown += (sender, e) => btn_changeClicked(sender, e, DailyQuestType.Fun);

            LoadStyle();

            this.Loaded += DailyQuestWindow_Loaded;
        }

        private void DailyQuestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormExecution.ClientConfig.DoTutoQuests)
            {
                BCA_TutoPopup tutopopup = new BCA_TutoPopup();
                maingrid.Children.Add(tutopopup);
                tutopopup.HorizontalAlignment = HorizontalAlignment.Center;
                tutopopup.VerticalAlignment = VerticalAlignment.Center;
                tutopopup.tuto_popup.IsOpen = true;
                tutopopup.tuto_popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
                tutopopup.tuto_popup.PlacementTarget = maingrid;
                tutopopup.SetText(StartDisclaimer.QuestsTutorial);
                tutopopup.tuto_popup.MaxWidth = FormExecution.GetChat().Width - 200;
                tutopopup.btnNext.Visibility = Visibility.Hidden;
                tutopopup.btnSkip.ButtonText = "Fin !";
                tutopopup.btnSkip.Update();

                tutopopup.SkipTuto += SkipTutorial;
                tutopopup.NextStep += Tutopopup_NextStep;
            }
        }

        private void Tutopopup_NextStep(BCA_TutoPopup popup)
        {
            SkipTutorial(popup);
        }

        private void SkipTutorial(BCA_TutoPopup popup)
        {
            popup.tuto_popup.IsOpen = false;
            LoadStyle();
            FormExecution.ClientConfig.DoTutoQuests = false;
            FormExecution.ClientConfig.Save();
        }

        private void _admin_ChangeQuest(DailyQuestType dqtype, string quest, bool savequest = false)
        {
            TextBlock target = tbNormalQuest;
            switch (dqtype)
            {
                case DailyQuestType.Normal:
                    target = tbNormalQuest;
                    if (savequest)
                        _quests[0] = quest;
                    break;
                case DailyQuestType.Special:
                    target = tbSpecialQuest;
                    if (savequest)
                        _quests[1] = quest;
                    break;
                case DailyQuestType.Fun:
                    target = tbFunQuest;
                    if (savequest)
                        _quests[2] = quest;
                    break;
            }
            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            target.RenderTransformOrigin = new Point(0.5, 0.5);
            target.RenderTransform = scale;

            DoubleAnimation growAnimationClose = new DoubleAnimation();
            growAnimationClose.Duration = TimeSpan.FromMilliseconds(100);
            growAnimationClose.From = 1.0;
            growAnimationClose.To = 0.0;
            storyboard.Children.Add(growAnimationClose);

            Storyboard.SetTargetProperty(growAnimationClose, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationClose, target);

            storyboard.Completed += (sender, e) => changeTextAnimation_Completed(sender, e, target, quest);
            storyboard.Begin();
        }

        private void changeTextAnimation_Completed(object sender, EventArgs e, TextBlock target, string quest)
        {
            target.Text = quest;
            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            target.RenderTransformOrigin = new Point(0.5, 0.5);
            target.RenderTransform = scale;

            DoubleAnimation growAnimationOpen = new DoubleAnimation();
            growAnimationOpen.Duration = TimeSpan.FromMilliseconds(100);
            growAnimationOpen.From = 0.0;
            growAnimationOpen.To = 1.0;
            storyboard.Children.Add(growAnimationOpen);
            Storyboard.SetTargetProperty(growAnimationOpen, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationOpen, target);

            storyboard.Begin();
        }

        private void _admin_ChangeQuestState(DailyQuestType dqtype, bool success)
        {
            if (!success)
                return;

            switch (dqtype)
            {
                case DailyQuestType.Normal:
                    SetQuestState(-1, border_normal, tbNormalQuest);
                    btn_getnormal.Visibility = Visibility.Hidden;
                    break;
                case DailyQuestType.Special:
                    SetQuestState(-1, border_special, tbSpecialQuest);
                    btn_getspecial.Visibility = Visibility.Hidden;
                    break;
                case DailyQuestType.Fun:
                    SetQuestState(-1, border_fun, tbFunQuest);
                    btn_getfun.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void btn_getClicked(object sender, MouseButtonEventArgs e, DailyQuestType dqtype)
        {
            _admin.SendGetReward(dqtype);
        }
        private void btn_changeClicked(object sender, MouseButtonEventArgs e, DailyQuestType dqtype)
        {
            _admin.SendChangeDailyQuest(dqtype);
        }

        private void SetQuestState(int state, Border bd, TextBlock tb)
        {
            if (state > 0)
                return;

            bd.BorderThickness = new Thickness(3);

            if (state < 0)
            {
                if (bd.BorderBrush != null)
                {
                    ColorAnimation border;
                    border = new ColorAnimation();
                    border.From = ((SolidColorBrush)bd.BorderBrush).Color;
                    border.To = Colors.Green;
                    border.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                    bd.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, border);
                    ColorAnimation background;
                    background = new ColorAnimation();
                    background.From = ((SolidColorBrush)bd.Background).Color;
                    background.To = Colors.LightGreen;
                    background.Duration = new Duration(TimeSpan.FromSeconds(1));
                    bd.Background.BeginAnimation(SolidColorBrush.ColorProperty, background);
                }
                else
                {
                    bd.BorderBrush = new SolidColorBrush(Colors.Green);
                    bd.Background = new SolidColorBrush(Colors.LightGreen);
                }
                Grid.SetColumnSpan(tb, 2);
            }
            else
            {
                bd.BorderBrush = new SolidColorBrush(Colors.Blue);
                bd.Background = new SolidColorBrush(Colors.LightSkyBlue);
            }
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> RankedButtons = new List<BCA_ColorButton>();
            RankedButtons.AddRange(new[] { btn_changefun, btn_changenormal, btn_changespecial, btn_getfun, btn_getnormal, btn_getspecial });

            AppDesignConfig style = FormExecution.AppDesignConfig;

            foreach (BCA_ColorButton btn in RankedButtons)
            {
                btn.Color1 = style.GetGameColor("Color1DailyQuest");
                btn.Color2 = style.GetGameColor("Color2DailyQuest");
                btn.Update();
            }
            this.FontFamily = style.Font;
        }
        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(0, 50, 50, 0);
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.bg_border.CornerRadius = new CornerRadius(0);
            }
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
