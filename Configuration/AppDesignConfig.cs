using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace hub_client.Configuration
{
    public class AppDesignConfig
    {
        public Dictionary<string, Color> GameColors;
        public FontFamily Font = new FontFamily("Dosis");
        public int FontSize = 22;

        public AppDesignConfig()
        {
            GameColors = new Dictionary<string, Color>();

            GameColors.Add("StandardMessageColor", Colors.Black);
            GameColors.Add("AnimationMessageColor", Colors.OrangeRed);
            GameColors.Add("InformationMessageColor", Colors.DarkBlue);
            GameColors.Add("LauncherMessageColor", Colors.DarkViolet);
            GameColors.Add("GreetMessageColor", Colors.ForestGreen);
            GameColors.Add("StaffMessageColor", Colors.Red);

            GameColors.Add("Color1HomeHeadButton", (Color)ColorConverter.ConvertFromString("#FF26164F"));

            GameColors.Add("Color2HomeHeadButton", (Color)ColorConverter.ConvertFromString("#FF221A29"));

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
        }
        public void UpdateResourcesDictionary()
        {

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

        public void Save()
        {
            File.WriteAllText(FormExecution.AppDesignConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}