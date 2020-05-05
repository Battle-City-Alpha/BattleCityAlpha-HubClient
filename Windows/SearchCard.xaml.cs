using BCA.Common;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Cards;
using hub_client.Configuration;
using hub_client.Enums;
using hub_client.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour SearchCard.xaml
    /// </summary>
    public partial class SearchCard : Window
    {
        List<CardInfos> filter_cards = new List<CardInfos>();
        private AppDesignConfig style = FormExecution.AppDesignConfig;

        private Dictionary<CardType, CardType[]> _cardsSecondTypes;

        public SearchCard()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            _cardsSecondTypes = new Dictionary<CardType, CardType[]>();
            _cardsSecondTypes.Add(CardType.Monstre, new CardType[] { CardType.Normal, CardType.Effet, CardType.Fusion, CardType.Rituelle, CardType.Synchro, CardType.Xyz, CardType.Pendule, CardType.Link, CardType.SpecialSummon, CardType.Syntoniseur, CardType.Union, CardType.Esprit, CardType.Gemini, CardType.Flip, CardType.Toon, CardType.Jeton, CardType.Monstre_Piège });
            _cardsSecondTypes.Add(CardType.Magie, new CardType[] { CardType.Magie, CardType.Jeu_Rapide, CardType.Continue, CardType.Rituelle, CardType.Equipement, CardType.Terrain });
            _cardsSecondTypes.Add(CardType.Piège, new CardType[] { CardType.Piège, CardType.Continue, CardType.Contre });

            cb_cardtype1.Items.Add(CardType.Monstre);
            cb_cardtype1.Items.Add(CardType.Magie);
            cb_cardtype1.Items.Add(CardType.Piège);
            cb_cardtype1.SelectionChanged += Cb_cardtype1_SelectionChanged;

            cb_cardtype2.IsEnabled = false;
            cb_cardattribute.IsEnabled = false;
            cb_cardrace.IsEnabled = false;

            foreach (CardRace race in Enum.GetValues(typeof(CardRace)))
                cb_cardrace.Items.Add(race);
            foreach (CardAttribute attribute in Enum.GetValues(typeof(CardAttribute)))
                cb_cardattribute.Items.Add(attribute);

            List<string> setnames = GlobalTools.GetDictionnaryValues(CardManager.SetCodes);
            setnames.Sort();
            foreach (string value in setnames)
                cb_setnames.Items.Add(value);

            LoadStyle();

            this.MouseDown += Window_MouseDown;
        }

        private void Cb_cardtype1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_cardtype1.SelectedIndex == 0)
            {
                cb_cardtype2.IsEnabled = false; 
                cb_cardtype1.SelectedIndex = 0;
                cb_cardtype2.SelectedIndex = 0;
                cb_cardattribute.SelectedIndex = 0;
                cb_cardrace.SelectedIndex = 0;
                return;
            }
            CardType selected = (CardType)cb_cardtype1.SelectedItem;
            cb_cardtype2.IsEnabled = true;
            cb_cardtype2.Items.Clear();
            foreach (CardType t in _cardsSecondTypes[selected])
                cb_cardtype2.Items.Add(t);
            cb_cardtype2.Items.Insert(0, "N/A");
            cb_cardtype2.SelectedIndex = 0;

            if (selected == CardType.Monstre)
            {
                cb_cardattribute.IsEnabled = true;
                cb_cardrace.IsEnabled = true;                
            }
            else
            {
                cb_cardattribute.IsEnabled = false;
                cb_cardrace.IsEnabled = false;
            }
        }

        private void LoadStyle()
        {
            List<BCA_ColorButton> Buttons = new List<BCA_ColorButton>();
            Buttons.AddRange(new[] { btn_found, btn_search });

            foreach (BCA_ColorButton btn in Buttons)
            {
                btn.Color1 = style.GetGameColor("Color1SearchCardButton");
                btn.Color2 = style.GetGameColor("Color2SearchCardButton");
                btn.Update();
            }

            this.FontFamily = style.Font;
        }

        private void BCA_ColorButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            filter_cards.Clear();
            lb_cards.Items.Clear();
            foreach (int card in CardManager.GetKeys().Where(card => CheckCard(CardManager.GetCard(card))))
            {
                filter_cards.Add(CardManager.GetCard(card));
                lb_cards.Items.Add(CardManager.GetCard(card).Name);
            }
        }

        public bool CheckCard(CardInfos infos)
        {
            if (cb_cardtype1.SelectedIndex != 0)
                if (!infos.GetCardTypes().Contains((CardType)cb_cardtype1.SelectedItem))
                    return false;
            if (cb_cardtype2.SelectedIndex != 0)
                if (!infos.GetCardTypes().Contains((CardType)cb_cardtype2.SelectedItem))
                    return false;
            if (cb_cardattribute.SelectedIndex != 0)
                if (infos.Attribute != ((int)(CardAttribute)cb_cardattribute.SelectedItem))
                    return false;
            if (cb_cardrace.SelectedIndex != 0)
                if (infos.Race != ((int)(CardRace)cb_cardrace.SelectedItem))
                    return false;
            if (cb_setnames.SelectedIndex != 0)
                if (!infos.GetSetName().Contains(cb_setnames.SelectedItem.ToString()))
                    return false;
            if (tb_level.Text != string.Empty)
                if (infos.Level != Convert.ToInt32(tb_level.Text))
                    return false;
            if (tb_attack.Text != string.Empty)
                if (infos.Atk != Convert.ToInt32(tb_attack.Text))
                    return false;
            if (tb_defence.Text != string.Empty)
                if (infos.Def != Convert.ToInt32(tb_defence.Text))
                    return false;
            if (tb_name.Text != string.Empty)
                if (!infos.Name.ToUpper().Contains(tb_name.Text.ToUpper()))
                    return false;

            return true;
        }

        private void lb_cards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count < 1)
                return;

            CardInfos card = filter_cards[lb_cards.Items.IndexOf(e.AddedItems[0])];

            DisplayCardInfo.SetCard(card);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cb_cardtype1.Items.Insert(0, "N/A");
            cb_cardtype2.Items.Insert(0, "N/A");
            cb_cardattribute.Items.Insert(0, "N/A");
            cb_cardrace.Items.Insert(0, "N/A");
            cb_setnames.Items.Insert(0, "N/A");

            cb_cardtype1.SelectedIndex = 0;
            cb_cardtype2.SelectedIndex = 0;
            cb_cardattribute.SelectedIndex = 0;
            cb_cardrace.SelectedIndex = 0;
            cb_setnames.SelectedIndex = 0;
        }

        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void btn_found_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lb_cards.SelectedItem == null)
                return;

            CardInfos card = filter_cards[lb_cards.Items.IndexOf(lb_cards.SelectedItem)];
            FormExecution.Client.Send(PacketType.SearchCard, new StandardClientSearchCard
            {
                Id = card.Id
            });
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
                this.bg_border.CornerRadius = new CornerRadius(40, 0, 40, 40);
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
