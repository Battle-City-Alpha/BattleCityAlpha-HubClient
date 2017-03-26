using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace hub_client.Configuration
{
    public class AppDesignConfig
    {
        public Color Color1HomeHeadButton = (Color)ColorConverter.ConvertFromString("#FF26164F");
        public Color Color2HomeHeadButton = (Color)ColorConverter.ConvertFromString("#FF221A29");
        public Color Color1HomeBottomButton = (Color)ColorConverter.ConvertFromString("#FF280E69");
        public Color Color2HomeBottomButton = (Color)ColorConverter.ConvertFromString("#FF410951");
        public Color Color1HomePlaceButton = (Color)ColorConverter.ConvertFromString("#FFEA1313");
        public Color Color2HomePlaceButton = (Color)ColorConverter.ConvertFromString("#FF5F0B08");
        public Color Color1HomeAnimationButton = (Color)ColorConverter.ConvertFromString("#FF149A2D");
        public Color Color2HomeAnimationButton = (Color)ColorConverter.ConvertFromString("#FF005613");


        public Color StandardMessageColor = Colors.Black;
        public Color AnimationMessageColor = Colors.OrangeRed;
        public Color InformationMessageColor = Colors.DarkBlue;
        public Color LauncherMessageColor = Colors.DarkViolet;
    }
}
