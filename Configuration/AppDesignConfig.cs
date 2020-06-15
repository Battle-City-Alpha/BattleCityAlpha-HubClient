using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

namespace hub_client.Configuration
{
    public class AppDesignConfig
    {
        public Dictionary<string, Color> GameColors;
        public FontFamily Font = new FontFamily("Arial");
        public int FontSize = 15;

        public double ShopWidth = -1;
        public double ShopHeight = -1;

        public AppDesignConfig()
        {
            GameColors = new Dictionary<string, Color>();

            GameColors.Add("StandardMessageColor", Colors.Black);
            GameColors.Add("AnimationMessageColor", Colors.OrangeRed);
            GameColors.Add("InformationMessageColor", Colors.DarkBlue);
            GameColors.Add("LauncherMessageColor", Colors.DarkViolet);
            GameColors.Add("GreetMessageColor", Colors.ForestGreen);
            GameColors.Add("StaffMessageColor", Colors.Red);
            GameColors.Add("DuelArenaMessageColor", Colors.Orange);

            GameColors.Add("CustomRoomColor", Colors.Red);
            GameColors.Add("ShadowRoomColor", Colors.DarkViolet);

            GameColors.Add("ChatBackgroundColor", Colors.White);
            GameColors.Add("ChatInputBackgroundColor", Colors.White);
            GameColors.Add("UserlistBackgroundColor", Colors.White);

            GameColors.Add("PopupPMBackgroundColor", (Color)ColorConverter.ConvertFromString("#FF341c4d"));
            GameColors.Add("PopupPMContentColor", Colors.White);
            GameColors.Add("PopupPMTitleColor", Colors.Yellow);
            GameColors.Add("PopupPMHeaderColor", (Color)ColorConverter.ConvertFromString("#FFa57daf"));

            GameColors.Add("NotificationColor", Colors.Red);
            GameColors.Add("AnimationSoonColor", Colors.Blue);

            GameColors.Add("HighlighMessageColor", (Color)ColorConverter.ConvertFromString("#AAf5f314"));

            GameColors.Add("CaptionDuelTextColor", (Color)ColorConverter.ConvertFromString("#ff0000ff"));

            GameColors.Add("Color1HomeHeadButton", (Color)ColorConverter.ConvertFromString("#FF26164F"));
            GameColors.Add("Color2HomeHeadButton", (Color)ColorConverter.ConvertFromString("#FF221A29"));

            GameColors.Add("Color1MonthlyBonusViewer", (Color)ColorConverter.ConvertFromString("#FF175A91"));
            GameColors.Add("Color2MonthlyBonusViewer", (Color)ColorConverter.ConvertFromString("#FF0FA2EE"));

            GameColors.Add("Color1DailyQuest", (Color)ColorConverter.ConvertFromString("#FF175A91"));
            GameColors.Add("Color2DailyQuest", (Color)ColorConverter.ConvertFromString("#FF0FA2EE"));

            GameColors.Add("Color1ArenaButton", (Color)ColorConverter.ConvertFromString("#FFF40202"));
            GameColors.Add("Color2ArenaButton", (Color)ColorConverter.ConvertFromString("#FFC20505"));

            GameColors.Add("Color1PanelButton", (Color)ColorConverter.ConvertFromString("#FF0068BC"));
            GameColors.Add("Color2PanelButton", (Color)ColorConverter.ConvertFromString("#FF01062E"));

            GameColors.Add("Color1ShopButton", (Color)ColorConverter.ConvertFromString("#FF202CB3"));
            GameColors.Add("Color2ShopButton", (Color)ColorConverter.ConvertFromString("#FF1666B1"));

            GameColors.Add("Color1CenterBrocanteButton", (Color)ColorConverter.ConvertFromString("#B7397E"));
            GameColors.Add("Color2CenterBrocanteButton", (Color)ColorConverter.ConvertFromString("#7E2757"));
            GameColors.Add("Color1BrocanteButton", (Color)ColorConverter.ConvertFromString("#B9AB06"));
            GameColors.Add("Color2BrocanteButton", (Color)ColorConverter.ConvertFromString("#F1E007"));

            GameColors.Add("Color1SearchCardButton", (Color)ColorConverter.ConvertFromString("#FFCD0000"));
            GameColors.Add("Color2SearchCardButton", (Color)ColorConverter.ConvertFromString("#FF880D0D"));

            GameColors.Add("Color1ToolsButton", (Color)ColorConverter.ConvertFromString("#EC814B"));
            GameColors.Add("Color2ToolsButton", (Color)ColorConverter.ConvertFromString("#934833"));

            GameColors.Add("Color1LoginButton", (Color)ColorConverter.ConvertFromString("#FF26164F"));
            GameColors.Add("Color2LoginButton", (Color)ColorConverter.ConvertFromString("#FF221A29"));

            GameColors.Add("Color1PopBoxButton", (Color)ColorConverter.ConvertFromString("#59ACFF"));
            GameColors.Add("Color2PopBoxButton", (Color)ColorConverter.ConvertFromString("#003D79"));

            GameColors.Add("Color1DuelRequestButton", (Color)ColorConverter.ConvertFromString("#F72115"));
            GameColors.Add("Color2DuelRequestButton", (Color)ColorConverter.ConvertFromString("#A80F06"));

            GameColors.Add("Color1TradeButton", (Color)ColorConverter.ConvertFromString("#59ACFF"));
            GameColors.Add("Color2TradeButton", (Color)ColorConverter.ConvertFromString("#003D79"));

            GameColors.Add("Color1SoloModeButton", (Color)ColorConverter.ConvertFromString("#FF321A1C"));
            GameColors.Add("Color2SolodeModeButton", (Color)ColorConverter.ConvertFromString("#FF6C3737"));

            GameColors.Add("Color1PrestigeShopButton", (Color)ColorConverter.ConvertFromString("#FF202CB3"));
            GameColors.Add("Color2PrestigeShopButton", (Color)ColorConverter.ConvertFromString("#FF1666B1"));

            GameColors.Add("Color1DataRetrievalButton", (Color)ColorConverter.ConvertFromString("#FF4F163F"));
            GameColors.Add("Color2DataRetrievalButton", (Color)ColorConverter.ConvertFromString("#FF1F1429"));

            GameColors.Add("Color1AnimationPlanning", (Color)ColorConverter.ConvertFromString("#FF4E0000"));
            GameColors.Add("Color2AnimationPlanning", (Color)ColorConverter.ConvertFromString("#FF420011"));

            GameColors.Add("Color1ShadowDuel", (Color)ColorConverter.ConvertFromString("#FF1114FF"));
            GameColors.Add("Color2ShadowDuel", (Color)ColorConverter.ConvertFromString("#FF061235"));
        }

        public Color GetGameColor(string propertyName)
        {
            if (GameColors.ContainsKey(propertyName))
                return GameColors[propertyName];
            return Colors.Black;
        }

        public bool SetGameColor(string propertyName, Color c)
        {
            if (GameColors.ContainsKey(propertyName))
            {
                GameColors[propertyName] = c;
                return true;
            }
            return false;
        }

        public System.Drawing.Color GetDrawingColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public void Save()
        {
            File.WriteAllText(FormExecution.AppDesignConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}