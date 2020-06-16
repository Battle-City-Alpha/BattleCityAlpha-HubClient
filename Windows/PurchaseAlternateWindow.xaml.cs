using BCA.Common;
using hub_client.Cards;
using hub_client.Enums;
using hub_client.Stuff;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour PurchaseAlternateWindow.xaml
    /// </summary>
    public partial class PurchaseAlternateWindow : Window
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private PurchaseAdministrator _admin;
        private PlayerCard[] cards;
        private BoosterInfo _infos;
        private int _indexdisplay;

        public PurchaseAlternateWindow(PurchaseAdministrator admin, BoosterInfo infos)
        {
            InitializeComponent(); 
            LoadStyle();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            this.label_booster_name.Content = infos.PurchaseTag + " : " + infos.Name;

            _admin = admin;
            _infos = infos;

            this.MouseDown += Window_MouseDown;
            this.Closed += PurchaseAlternateWindow_Closed;

            _admin.PurchaseItem += _admin_PurchaseItem;
        }

        private void PurchaseAlternateWindow_Closed(object sender, EventArgs e)
        {
            _admin.PurchaseItem -= _admin_PurchaseItem;
        }

        private void _admin_PurchaseItem(int[] cards)
        {
            UpdateCards(cards);
            SaveDeck((_infos.Type == PurchaseType.Demarrage || _infos.Type == PurchaseType.Structure));
            StartCardAnimation();
        }
        public void UpdateCards(int[] list)
        {
            Dictionary<int, PlayerCard> dcards = new Dictionary<int, PlayerCard>();
            for (int i = 0; i < list.Count(); i++)
            {
                CardInfos infos = CardManager.GetCard(list[i]);
                if (infos == null)
                {
                    infos = new CardInfos();
                    infos.Id = list[i];
                    infos.Name = "Id inconnu :" + infos.Id;
                }
                if (!dcards.ContainsKey(infos.Id))
                    dcards.Add(infos.Id, new PlayerCard { Id = infos.Id, Name = infos.Name, Quantity = 1 });
                else
                    dcards[infos.Id].Quantity++;
            }
            cards = dcards.Values.ToArray();
            cards = cards.OrderBy(x => x.Name).ToArray();
        }
        private void SaveDeck(bool isStructureOrStartingDeck)
        {
            List<int> main = new List<int>();
            List<int> extra = new List<int>();

            foreach (PlayerCard pcard in cards)
            {
                CardInfos card = CardManager.GetCard(pcard.Id);
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

        private void StartCardAnimation()
        {
            if (_admin.Client.PlayerManager.Collections.Count == 0)
                _admin.Client.PlayerManager.LoadCoffre();
            _indexdisplay = 0;
            TranslateCard(cards[_indexdisplay]);
        }
        private void TranslateCard(PlayerCard card)
        {
            BCA_BoosterCard bca_bc = new BCA_BoosterCard(card, !_admin.Client.PlayerManager.Collections.ContainsKey(card.Id));
            bca_bc.Margin = new Thickness(3);
            _admin.Client.PlayerManager.AddCard(card.Id, card.Quantity);
            bca_bc.SetImageSource(FormExecution.AssetsManager.GetImage("Sleeves", "203"));

            wp_cards.Children.Add(bca_bc);

            Storyboard storyboard = new Storyboard();

            TranslateTransform translation = new TranslateTransform(0.0, 0.0);
            bca_bc.RenderTransformOrigin = new Point(0.5, 0.5);
            bca_bc.RenderTransform = translation;

            DoubleAnimation translateXAnimation = new DoubleAnimation();
            translateXAnimation.Duration = TimeSpan.FromMilliseconds(200);
            translateXAnimation.From = -500;
            translateXAnimation.To = 0;
            storyboard.Children.Add(translateXAnimation);

            Storyboard.SetTargetProperty(translateXAnimation, new PropertyPath("RenderTransform.X"));
            Storyboard.SetTarget(translateXAnimation, bca_bc);

            storyboard.Completed += (senderA, eA) => Storyboard_Completed(senderA, eA, bca_bc, card.Id);
            storyboard.Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e, BCA_BoosterCard img_card, int id)
        {
            if (_indexdisplay != cards.Length - 1)
            {
                _indexdisplay++;
                TranslateCard(cards[_indexdisplay]);
            }

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

            storyboard.Completed += (senderA, eA) => DisplayFinish(senderA, eA, img_card, id);
            storyboard.Begin();
        }
        private void DisplayFinish(object sender, EventArgs e, BCA_BoosterCard img_card, int id)
        {
            if (File.Exists(System.IO.Path.Combine(FormExecution.path, "BattleCityAlpha", "pics", id.ToString() + ".jpg")))
                img_card.SetImageSource(FormExecution.AssetsManager.GetPics(new string[] { "BattleCityAlpha", "pics", id.ToString() + ".jpg" }));

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

            storyboard.Begin();
        }
        

        private void LoadStyle()
        {
            this.FontFamily = FormExecution.AppDesignConfig.Font;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.ActivateShop();
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
