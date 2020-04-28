using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Stuff;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Purchase.xaml
    /// </summary>
    public partial class Purchase : Window
    {
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        PurchaseAdministrator _admin;
        CardInfos[] cards;
        ToolTip tip = new ToolTip();
        private BoosterInfo _infos;
        private int index_show = 0;

        public Purchase(PurchaseAdministrator admin, BoosterInfo infos)
        {
            InitializeComponent();
            LoadStyle();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _admin = admin;
            _admin.PurchaseItem += _admin_PurchaseItem;

            this.FontFamily = FormExecution.AppDesignConfig.Font;
            this.MouseDown += Window_MouseDown;

            _infos = infos;

            btn_next.IsEnabled = false;
            btn_all.IsEnabled = false;
            btn_next.Visibility = Visibility.Hidden;
            btn_all.Visibility = Visibility.Hidden;

            btn_next.MouseLeftButtonDown += Btn_next_MouseLeftButtonDown;
            btn_all.MouseLeftButtonDown += Btn_all_MouseLeftButtonDown;
        }

        private void Btn_all_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btn_next.IsEnabled = false;
            btn_all.IsEnabled = false;
            btn_next.Visibility = Visibility.Hidden;
            btn_all.Visibility = Visibility.Hidden;
            Grid.SetRowSpan(img_card, 2);
            img_card.Margin = new Thickness(20);

            lb_cards.Items.Clear();
            foreach (CardInfos infos in cards)
                lb_cards.Items.Add(infos.Name);


            lb_cards.Items.SortDescriptions.Add(
            new System.ComponentModel.SortDescription("",
            System.ComponentModel.ListSortDirection.Ascending));
        }

        private void Btn_next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationDisplayCard(cards[index_show].Id);
            index_show++;

            if (index_show == cards.Length)
            {
                btn_next.IsEnabled = false;
                btn_all.IsEnabled = false;
                btn_next.Visibility = Visibility.Hidden;
                btn_all.Visibility = Visibility.Hidden;
                Grid.SetRowSpan(img_card, 2);
                img_card.Margin = new Thickness(20);
            }
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_all, btn_next });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ShopButton");
                btn.Color2 = style.GetGameColor("Color2ShopButton");
                btn.Update();
            }

            this.FontFamily = style.Font;
        }

        public void UpdateCards(int[] list)
        {
            cards = new CardInfos[list.Count()];
            for (int i = 0; i < list.Count(); i++)
            {
                CardInfos infos = CardManager.GetCard(list[i]);
                cards[i] = infos;
                if (infos == null)
                {
                    infos.Id = list[i];
                    infos.Name = "Id inconnu :" + infos.Id;
                }
            }

            AnimationDisplayCard(cards[index_show].Id);
            index_show++;

            btn_next.IsEnabled = true;
            btn_all.IsEnabled = true;
            btn_next.Visibility = Visibility.Visible;
            btn_all.Visibility = Visibility.Visible;
        }

        private void SaveDeck(bool isStructureOrStartingDeck)
        {
            List<int> main = new List<int>();
            List<int> extra = new List<int>();

            foreach (CardInfos card in cards)
            {
                if (card.GetCardTypes().Contains(CardType.Synchro) || card.GetCardTypes().Contains(CardType.Xyz) || card.GetCardTypes().Contains(CardType.Fusion) || card.GetCardTypes().Contains(CardType.Link))
                {
                    if (!extra.Contains(card.Id))
                        extra.Add(card.Id);
                }
                else
                {
                    if (!main.Contains(card.Id))
                        main.Add(card.Id);
                }
            }

            string deck = "#created by purchase " + _infos.PurchaseTag;
            deck += Environment.NewLine + "#main";
            foreach (int id in main)
                deck += Environment.NewLine + id.ToString();
            deck += Environment.NewLine + "#extra";
            foreach (int id in extra)
                deck += Environment.NewLine + id.ToString();
            deck += Environment.NewLine + "!side";

            if (isStructureOrStartingDeck)
                File.WriteAllText(System.IO.Path.Combine(FormExecution.path, "BattleCityAlpha", "deck", _infos.Name + ".ydk"), deck);
            else
                File.WriteAllText(System.IO.Path.Combine(FormExecution.path, "BattleCityAlpha", "deck", "new_cards.ydk"), deck);
        }

        private void _admin_PurchaseItem(int[] list)
        {
            UpdateCards(list);
            SaveDeck((_infos.Type == PurchaseType.Demarrage || _infos.Type == PurchaseType.Structure));
        }

        private void lb_cards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CardInfos infos = null;
            foreach (CardInfos card in cards)
                if (lb_cards.SelectedItem != null && card.Name == lb_cards.SelectedItem.ToString())
                {
                    infos = card;
                    break;
                }

            if (infos != null)
                img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", infos.Id.ToString() + ".jpg" });
        }

        private void AnimationDisplayCard(int id)
        {
            img_card.Source = FormExecution.AssetsManager.GetImage("Sleeves", "203");

            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            img_card.RenderTransformOrigin = new Point(0.5, 0.5);
            img_card.RenderTransform = scale;

            DoubleAnimation growAnimationClose = new DoubleAnimation();
            growAnimationClose.Duration = TimeSpan.FromMilliseconds(200);
            growAnimationClose.From = 1.0;
            growAnimationClose.To = 0.0;
            storyboard.Children.Add(growAnimationClose);

            Storyboard.SetTargetProperty(growAnimationClose, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationClose, img_card);

            storyboard.Completed += (senderA, eA) => Storyboard_Completed(senderA, eA, id);
            storyboard.Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e, int id)
        {
            lb_cards.SelectedIndex = index_show - 1;
            img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", id.ToString() + ".jpg" });

            Storyboard storyboard = new Storyboard();

            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            img_card.RenderTransformOrigin = new Point(0.5, 0.5);
            img_card.RenderTransform = scale;

            DoubleAnimation growAnimationOpen = new DoubleAnimation();
            growAnimationOpen.Duration = TimeSpan.FromMilliseconds(200);
            growAnimationOpen.From = 0.0;
            growAnimationOpen.To = 1.0;
            storyboard.Children.Add(growAnimationOpen);
            Storyboard.SetTargetProperty(growAnimationOpen, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(growAnimationOpen, img_card);

            storyboard.Completed += DisplayFinish;

            storyboard.Begin();
        }

        private void DisplayFinish(object sender, EventArgs e)
        {
            lb_cards.Items.Add(cards[index_show - 1].Name);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.PurchaseItem -= _admin_PurchaseItem;
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
