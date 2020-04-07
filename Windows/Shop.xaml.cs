using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Helpers;
using hub_client.Stuff;
using hub_client.Windows.Controls;
using hub_client.WindowsAdministrator;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Shop.xaml
    /// </summary>
    public partial class Shop : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        public ShopAdministrator _admin;

        BoosterInfo BoosterChoosen;

        public Shop(ShopAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;

            _admin.UpdateBoosterInfo += UpdateBoosterInfo;
            _admin.UpdateBattlePoints += _admin_UpdateBattlePoints;

            cbBooster.Items.Add("Booster");
            cbBooster.Items.Add("Booster Pack");
            cbBooster.Items.Add("Tournoi Pack");
            cbBooster.Items.Add("Pack du duelliste");
            cbBooster.Items.Add("Arsenal Mystérieux");
            cbBooster.Items.Add("Gold Premium");
            cbBooster.Items.Add("Deck de démarrage");
            cbBooster.Items.Add("Deck de structure");
            cbBooster.Items.Add("Battle Pack");

            tb_searchBooster.tbChat.TextChanged += TbChat_TextChanged;

            LoadStyle();
        }

        private void TbChat_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<string> BoosterSelect = new List<string>();
            foreach (string name in BoosterManager.BoosterEditionList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            foreach (string name in BoosterManager.BoosterPackList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            foreach (string name in BoosterManager.PackDuellisteList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            foreach (string name in BoosterManager.ArsenalMysterieuxList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            foreach (string name in BoosterManager.DeckDeDemarrageList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            foreach (string name in BoosterManager.DeckStructureList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            foreach (string name in BoosterManager.GoldPremiumList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            foreach (string name in BoosterManager.BattlePackList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            foreach (string name in BoosterManager.TournoiList)
                if (name.ToLower().Contains(tb_searchBooster.GetText().ToLower()))
                    BoosterSelect.Add(name);
            lb_booster.Items.Clear();
            foreach (string name in BoosterSelect)
                lb_booster.Items.Add(name);
            if (tb_searchBooster.GetText() == "")
            {
                lb_booster.Items.Clear();
                switch (cbBooster.Text)
                {
                    case "Booster":
                        foreach (string item in BoosterManager.BoosterEditionList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Booster Pack":
                        foreach (string item in BoosterManager.BoosterPackList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Pack Du Duelliste":
                        foreach (string item in BoosterManager.PackDuellisteList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Arsenal Mystérieux":
                        foreach (string item in BoosterManager.ArsenalMysterieuxList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Deck De Démarrage":
                        foreach (string item in BoosterManager.DeckDeDemarrageList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Deck De Structure":
                        foreach (string item in BoosterManager.DeckStructureList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Gold Premium":
                        foreach (string item in BoosterManager.GoldPremiumList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Battle Pack":
                        foreach (string item in BoosterManager.BattlePackList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Tournoi Pack":
                        foreach (string item in BoosterManager.TournoiList)
                            lb_booster.Items.Add(item);
                        break;
                    case "Booster Spéciaux":
                        foreach (string item in BoosterManager.BoosterSpecial)
                            lb_booster.Items.Add(item);
                        break;
                }
            }
        }

        private void _admin_UpdateBattlePoints(int points)
        {
            tb_bps.Text = points.ToString();
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_prestigeshop, btn_purchase, btn_searchcard });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1ShopButton");
                btn.Color2 = style.GetGameColor("Color2ShopButton");
                btn.Update();
            }

            this.FontFamily = style.Font;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lb_booster.Items.Clear();
            foreach (string item in BoosterManager.BoosterEditionList)
                lb_booster.Items.Add(item);
        }

        private void cbBooster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lb_booster.Items.Clear();
            string newitem = e.AddedItems[0].ToString();
            switch (newitem)
            {
                case "Booster":
                    foreach (string item in BoosterManager.BoosterEditionList)
                        lb_booster.Items.Add(item);
                    break;
                case "Booster Pack":
                    foreach (string item in BoosterManager.BoosterPackList)
                        lb_booster.Items.Add(item);
                    break;
                case "Pack du duelliste":
                    foreach (string item in BoosterManager.PackDuellisteList)
                        lb_booster.Items.Add(item);
                    break;
                case "Arsenal Mystérieux":
                    foreach (string item in BoosterManager.ArsenalMysterieuxList)
                        lb_booster.Items.Add(item);
                    break;
                case "Deck de démarrage":
                    foreach (string item in BoosterManager.DeckDeDemarrageList)
                        lb_booster.Items.Add(item);
                    break;
                case "Deck de structure":
                    foreach (string item in BoosterManager.DeckStructureList)
                        lb_booster.Items.Add(item);
                    break;
                case "Gold Premium":
                    foreach (string item in BoosterManager.GoldPremiumList)
                        lb_booster.Items.Add(item);
                    break;
                case "Battle Pack":
                    foreach (string item in BoosterManager.BattlePackList)
                        lb_booster.Items.Add(item);
                    break;
                case "Tournoi Pack":
                    foreach (string item in BoosterManager.TournoiList)
                        lb_booster.Items.Add(item);
                    break;
                case "Booster Spéciaux":
                    foreach (string item in BoosterManager.BoosterSpecial)
                        lb_booster.Items.Add(item);
                    break;
            }
        }

        private void lb_booster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                LoadPurchase(lb_booster.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                logger.Warn("SHOP WARNING - {0}", ex.ToString());
                return;
            }
        }

        private void LoadPurchase(string Item)
        {
            BoosterChoosen = BoosterManager.InitializeBooster(Item);
            _admin.AskBooster(BoosterChoosen.PurchaseTag);
            tb_numberpack.Text = "1";
            tb_date.Text = BoosterChoosen.Date + ".";
            if (BoosterChoosen.Type == PurchaseType.Demarrage)
                tb_numbercardpack.Text = "C'est un deck de Démarrage.";
            if (BoosterChoosen.Type == PurchaseType.Structure)
                tb_numbercardpack.Text = "C'est un deck de Structure.";

            tb_card1.Text = BoosterChoosen.MostImportantCards[0];
            tb_card2.Text = BoosterChoosen.MostImportantCards[1];
            tb_card3.Text = BoosterChoosen.MostImportantCards[2];
            tb_card4.Text = BoosterChoosen.MostImportantCards[3];

            img_booster.Source = FormExecution.AssetsManager.GetImage(new string[] { "Booster", "pics", BoosterChoosen.PurchaseTag + ".jpg" });
        }

        public void UpdateBoosterInfo(int cardgot, int totalcard, int price, int cardperpack, int bp)
        {
            tb_cardgot.Text = cardgot.ToString();
            tb_totalcard.Text = totalcard.ToString();
            tb_price.Text = price.ToString();
            tb_numbercardpack.Text = cardperpack.ToString();
            tb_bps.Text = bp.ToString();

            int numberPacket = Convert.ToInt32(tb_numberpack.Text);
            if (numberPacket > 24)
                numberPacket = 24;
            if (numberPacket < 0)
                numberPacket = 0;
            tb_realprice.Text = (numberPacket * BoosterChoosen.Price).ToString();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void tb_numberpack_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int numberPacket = Convert.ToInt32(tb_numberpack.Text);
                if (numberPacket > 24)
                    numberPacket = 24;
                if (numberPacket < 0)
                    numberPacket = 0;
                tb_realprice.Text = (numberPacket * BoosterChoosen.Price).ToString();
                tb_numberpack.Text = numberPacket.ToString();
            }
            catch { tb_numbercardpack.Text = "1";  };            
        }

        private void btn_purchase_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int numberPacket = Convert.ToInt32(tb_numberpack.Text);
            _admin.Purchase(BoosterChoosen.PurchaseTag, numberPacket );
            FormExecution.OpenPurchase(BoosterChoosen.Name);
        }
        private void btn_searchcard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SearchCard form = new SearchCard();
            form.Show();
        }
        private void btn_brocante_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FormExecution.OpenBrocante();
        }
        private void btn_prestigeshop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PrestigeShop shop = new PrestigeShop();
            shop.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _admin.UpdateBoosterInfo -= UpdateBoosterInfo;
            _admin.UpdateBattlePoints -= _admin_UpdateBattlePoints;
        }

    }
}
