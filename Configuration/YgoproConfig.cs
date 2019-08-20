using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Configuration
{
    public class YgoproConfig
    {
        public string Roominfo;
        public bool Enabled3D = false;
        public int Antialias = 0;
        public string Username = "";
        public string DefaultDeck = "";
        public string GameFont = "simhei.ttf";
        public int FontSize = 16;
        public bool AutoPlacing = true;
        public bool RandomPlacing = false;
        public bool AutoChain = true;
        public bool NoChainDelay = false;
        public bool EnableCustomSleeves = true;
        public bool MuteOpponent = false;
        public bool MuteSpectators = true;
        public bool SaveReplay = false;
        public bool EnableSound = true;
        public int SoundVolume = 50;
        public bool EnableMusic = true;
        public int MusicVolume = 50;
        public bool HideSetname = true;
        
        public YgoproConfig()
        {

        }

        public void updateconfig(string roominfo, string username)
        {
            Roominfo = roominfo;
            Username = username;
            Save();
        }

        public void Save()
        {
            if ((File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.conf"))))
                File.Delete(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.conf"));

            StreamWriter writer = new StreamWriter(Path.Combine(FormExecution.path, "BattleCityAlpha", "system.CONF"));

            writer.WriteLine("#config file");
            writer.WriteLine("#nickname & gamename should be less than 20 characters");
            writer.WriteLine("#Generated using " + Roominfo);
            writer.WriteLine("use_d3d = " + Convert.ToInt32(Enabled3D));
            writer.WriteLine("allow_resize = 1");
            writer.WriteLine(("antialias = " + Antialias));
            writer.WriteLine("errorlog = 1");
            writer.WriteLine(("nickname = " + Username));
            writer.WriteLine("gamename =");
            writer.WriteLine(("lastdeck = " + DefaultDeck));
            writer.WriteLine("textfont = fonts/" + GameFont + " " + FontSize);
            writer.WriteLine("numfont = fonts/arialbd.ttf");
            writer.WriteLine(("serverport = " + Roominfo));
#if DEBUG
            writer.WriteLine(("lasthost = 127.0.0.1"));
#endif
#if !DEBUG
            writer.WriteLine(("lasthost = 185.212.225.85"));
#endif
            writer.WriteLine(("lastport = " + Roominfo));
            writer.WriteLine("autopos = " + Convert.ToInt32(AutoPlacing));
            writer.WriteLine("randompos = " + Convert.ToInt32(RandomPlacing));
            writer.WriteLine("autochain = " + Convert.ToInt32(AutoChain));
            writer.WriteLine("waitchain = " + Convert.ToInt32(NoChainDelay));
            writer.WriteLine("ignore1 = " + Convert.ToInt32(MuteOpponent));
            writer.WriteLine("ignore2 = " + Convert.ToInt32(MuteSpectators));
            writer.WriteLine(("hide_setname = " + Convert.ToInt32(HideSetname)));
            writer.WriteLine(("enable_sound = " + Convert.ToInt32(EnableSound)));
            writer.WriteLine(("sound_volume = " + Convert.ToDouble(SoundVolume)));
            writer.WriteLine(("enable_music = " + Convert.ToInt32(EnableMusic)));
            writer.WriteLine(("music_volume = " + Convert.ToDouble(MusicVolume)));
            writer.WriteLine("save_last_replay = " + Convert.ToInt32(SaveReplay));
            writer.Close();
        }
    }
}
