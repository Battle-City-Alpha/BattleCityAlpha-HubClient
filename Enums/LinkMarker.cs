using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Enums
{
    public enum LinkMarker
    {
        BottomLeft = 1,
        Bottom = 2,
        BottomRight = 4,
        Left = 8,
        Middle = 16, // Only used for displaying
        Right = 32,
        TopLeft = 64,
        Top = 128,
        TopRight = 256
    }
}
