using hub_client.Enums;

namespace hub_client.Stuff
{
    public class BoosterInfo
    {
        public string Name;
        public string Date;
        public string PurchaseTag;
        public string[] MostImportantCards;
        public int CardsNumber;
        public int CardsIHave;
        public string Cover;
        public PurchaseType Type;
        public int Price;

        public override string ToString()
        {
            return Name + " (" + PurchaseTag + ")";
        }
    }
}