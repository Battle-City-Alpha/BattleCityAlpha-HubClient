using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Stuff;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using NLog;
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
        private Logger logger = LogManager.GetCurrentClassLogger();

        private AppDesignConfig style = FormExecution.AppDesignConfig;
        PurchaseAdministrator _admin;
        CardInfos[] cards;
        ToolTip tip = new ToolTip();
        private BoosterInfo _infos;
        private int index_show = 0;

        private CardInfos _cardShow = null;

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

            img_card.MouseEnter += Img_card_MouseEnter;
            img_card.MouseLeave += Img_card_MouseLeave;
            cardinfos_popup.MouseEnter += Cardinfos_popup_MouseEnter;
        }

        private void Cardinfos_popup_MouseEnter(object sender, MouseEventArgs e)
        {
            if (cardinfos_popup.IsOpen)
                cardinfos_popup.IsOpen = true;
        }

        private void Img_card_MouseLeave(object sender, MouseEventArgs e)
        {
            this.cardinfos_popup.IsOpen = false;
        }

        private void Img_card_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                SetCard(_cardShow);
                this.cardinfos_popup.IsOpen = true;
            }
            catch (Exception ex)
            {
                logger.Error("POPUP PURCHASE - {0}", ex.ToString());
            }
        }

        private void SetCard(CardInfos card)
        {
            tb_cardname.Text = card.Name;
            CardType[] typeArray = card.GetCardTypes();
            string level = "";
            string atkdef = "";
            string attribute = "";
            if (typeArray.Contains(CardType.Magie) || typeArray.Contains(CardType.Piège))
            {

            }
            else
            {
                if (!typeArray.Contains(CardType.Link))
                {
                    if (typeArray.Contains(CardType.Pendule))
                        level = string.Format("◊{0}    {1}◊", card.LScale, card.RScale);
                    else
                        level = card.Level + "★";

                    atkdef = string.Format("{0}/{1}", card.Atk, card.Def);
                }
                else
                {
                    LinkMarker[] markers = card.GetLinkMarkers();
                    atkdef = card.Atk + "/LINK-" + markers.Count();

                    level = GetStringLinksMarkers(markers);
                }
                attribute = string.Format("{0}|{1}", card.GetRace(), card.GetAttribute());
            }
            tb_cardlevel.Text = level;
            tb_cardatkdef.Text = atkdef;
            tb_cardattribute.Text = attribute;
            tb_cardtype.Text = card.GetCardType();
            tb_carddesc.Text = card.Description;

        }
        private string GetStringLinksMarkers(IEnumerable<LinkMarker> types)
        {
            string toReturn = "";
            foreach (var linkmarker in types)
            {
                switch (linkmarker)
                {
                    case LinkMarker.BottomLeft:
                        toReturn += "[↙]";
                        break;
                    case LinkMarker.Bottom:
                        toReturn += "[↓]";
                        break;
                    case LinkMarker.BottomRight:
                        toReturn += "[↘]";
                        break;
                    case LinkMarker.Left:
                        toReturn += "[←]";
                        break;
                    case LinkMarker.Right:
                        toReturn += "[→]";
                        break;
                    case LinkMarker.TopLeft:
                        toReturn += "[↖]";
                        break;
                    case LinkMarker.Top:
                        toReturn += "[↑]";
                        break;
                    case LinkMarker.TopRight:
                        toReturn += "[↗]";
                        break;
                }
            }
            return toReturn;
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
                if (infos == null)
                {
                    infos = new CardInfos();
                    infos.Id = list[i];
                    infos.Name = "Id inconnu :" + infos.Id;
                }
                cards[i] = infos;
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
            _cardShow = null;
            foreach (CardInfos card in cards)
                if (lb_cards.SelectedItem != null && card.Name == lb_cards.SelectedItem.ToString())
                {
                    _cardShow = card;
                    break;
                }

            if (_cardShow != null)
                img_card.Source = FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", _cardShow.Id.ToString() + ".jpg" });
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
            if (File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "pics", id.ToString() + ".jpg")))
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation translateY = new DoubleAnimation();
            translateY.Duration = TimeSpan.FromMilliseconds(100);
            translateY.From = 450;
            translateY.To = 0;

            translateTransformBgBorder.BeginAnimation(TranslateTransform.YProperty, translateY);
        }
    }
}
