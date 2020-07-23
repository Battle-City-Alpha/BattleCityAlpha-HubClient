using BCA.Common;
using BCA.Common.Enums;
using hub_client.Configuration;
using NLog;
using System.Diagnostics;
using System.IO;

namespace hub_client.Helpers
{
    public static class YgoProHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Customization _bot_avatar = new Customization(CustomizationType.Avatar, 406, false, "");
        private static Customization _bot_border = new Customization(CustomizationType.Border, 26, false, "");
        private static Customization _bot_sleeve = new Customization(CustomizationType.Sleeve, 64, false, "");
        private static Customization _bot_partner = new Customization(CustomizationType.Partner, 127, false, "");
        private const int _defaultPort = 1111;
        private const string _defaultHost = "127.0.0.1";

        public static void LaunchYgoPro(string commandline)
        {
            Process Game = new Process();
            Game.StartInfo.FileName = Path.Combine(FormExecution.path, "BattleCityAlpha", "BCA.exe");
            Game.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path, "BattleCityAlpha");
            Game.StartInfo.Arguments = commandline;
            Game.Start();
        }
        private static void LaunchWindbot(string deck, string host = _defaultHost, int port = _defaultPort, int version = 0x1340, string dialog = "fr-FR", string name = "Kaibot")
        {
            string info = string.Format("Name={0}  Deck={1} Dialog={2} Host={3} Port={4} Version={5}", name, deck, dialog, host, port, version);

            logger.Trace("Windbot start with : " + info);

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = Path.Combine(FormExecution.path, "BattleCityAlpha", "Kaibot", "WindBot.exe");
            p.StartInfo.WorkingDirectory = Path.Combine(FormExecution.path, "BattleCityAlpha", "Kaibot");
            p.StartInfo.Arguments = info;
            p.Start();
        }

        public static void LaunchGame(Room room, string commandline)
        {
            for (int i = 0; i < 4; i++)
            {
                if (File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "avatars", "a_" + i + ".png")))
                    File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "avatars", "a_" + i + ".png"));
                if (File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "borders", "b_" + i + ".png")))
                    File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "borders", "b_" + i + ".png"));
                if (File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + i + ".png")))
                    File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + i + ".png"));
                if (File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "partners", "p_" + i + ".png")))
                    File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "partners", "p_" + i + ".png"));
                if (File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "emblems", "e_" + i + ".png")))
                    File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "emblems", "e_" + i + ".png"));
            }

            LaunchYgoPro(commandline);
        }
        public static void LaunchGameAgainstBot()
        {
            YgoproConfig.UpdateForced(false);
            UpdateAvatar(FormExecution.PlayerInfos.Avatar, 0);
            UpdateAvatar(_bot_avatar, 1);
            UpdateBorder(_bot_border, 1);
            UpdateSleeve(_bot_sleeve, 1);
            UpdateSleeve(_bot_partner, 1);

            LaunchYgoPro(string.Format("-b"));
        }

        public static void LoadCustomization(Customization avatar, Customization border, Customization sleeve, Customization partner, int team, string teamemblem, int pos)
        {
            UpdateAvatar(avatar, pos);
            UpdateBorder(border, pos);
            UpdateSleeve(sleeve, pos);
            UpdatePartner(partner, pos);
            UpdateTeamEmblem(team, teamemblem, pos);
        }
        private static void UpdateAvatar(Customization avatar, int i)
        {
            if (!File.Exists(Path.Combine(FormExecution.path, "Assets", "Avatars", avatar.Id + ".png")))
                FormExecution.AssetsManager.GetCustom(avatar);
            CopyAvatarToTexturesFolder(avatar, i);
        }
        private static void UpdateBorder(Customization border, int i)
        {
            if (!File.Exists(Path.Combine(FormExecution.path, "Assets", "Borders", border.Id + ".png")))
                FormExecution.AssetsManager.GetCustom(border);
            CopyBorderToTexturesFolder(border, i);
        }
        private static void UpdateSleeve(Customization sleeve, int i)
        {
            if (!File.Exists(Path.Combine(FormExecution.path, "Assets", "Sleeves", sleeve.Id + ".png")))
                FormExecution.AssetsManager.GetCustom(sleeve);
            CopySleeveToTexturesFolder(sleeve, i);
        }
        private static void UpdatePartner(Customization partner, int i)
        {
            if (!File.Exists(Path.Combine(FormExecution.path, "Assets", "Partners", partner.Id + ".png")))
                FormExecution.AssetsManager.GetCustom(partner);
            CopyPartnerToTexturesFolder(partner, i);
        }
        private static void UpdateTeamEmblem(int team, string emblem, int i)
        {
            if (!Directory.Exists(Path.Combine(FormExecution.path, "Assets", "Team")))
                Directory.CreateDirectory(Path.Combine(FormExecution.path, "Assets", "Team"));

            if (team == 0)
                return;

            if (!File.Exists(Path.Combine(FormExecution.path, "Assets", "Team", team + ".png")))
                FormExecution.AssetsManager.GetTeamEmblem(team, emblem);
            CopyEmblemToTexturesFolder(team, i);
        }

        private static void CopyAvatarToTexturesFolder(Customization avatar, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Avatars", avatar.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "avatars", "a_" + index.ToString() + ".png"), true);
        }
        private static void CopyBorderToTexturesFolder(Customization border, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Borders", border.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "borders", "b_" + index.ToString() + ".png"), true);
        }
        private static void CopySleeveToTexturesFolder(Customization sleeve, int index)
        {
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Sleeves", sleeve.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "sleeves", "s_" + index.ToString() + ".png"), true);
        }
        private static void CopyPartnerToTexturesFolder(Customization partner, int index)
        {
            if (!Directory.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "partners")))
                Directory.CreateDirectory(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "partners"));
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Partners", partner.Id.ToString() + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "partners", "p_" + index.ToString() + ".png"), true);
        }
        private static void CopyEmblemToTexturesFolder(int teamID, int index)
        {
            if (!Directory.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "emblems")))
                Directory.CreateDirectory(Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "emblems"));
            File.Copy(Path.Combine(FormExecution.path, "Assets", "Team", teamID + ".png"), Path.Combine(FormExecution.path, "BattleCityAlpha", "textures", "emblems", "e_" + index.ToString() + ".png"), true);
        }
    }
}
