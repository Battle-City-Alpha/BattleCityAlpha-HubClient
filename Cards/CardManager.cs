using NLog;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static hub_client.Cards.SQLCommands;

namespace hub_client.Cards
{
    public static class CardManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static event Action LoadingFinished;
        public static event Action<int, int> LoadingProgress;

        private static Dictionary<int, CardInfos> CardData = new Dictionary<int, CardInfos>();
        public static Dictionary<int, string> SetCodes = new Dictionary<int, string>();
        public static Dictionary<char, List<string>> SetCodesString = new Dictionary<char, List<string>>();

        private static HashSet<int> PicsID;
        private static int progress = 0;
        private static int total;

        private static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
                              TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }


        private static void LoadPicsFile()
        {
            if (!Directory.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "pics")))
                Directory.CreateDirectory(Path.Combine(FormExecution.path, "BattleCityAlpha", "pics"));
            string[] ids = Directory.GetFiles(Path.Combine(FormExecution.path, "BattleCityAlpha", "pics")).Select(file => Path.GetFileName(file).Split('.')[0]).ToArray();

            PicsID = new HashSet<int>();
            foreach (string id in ids)
            {
                int i;
                if (int.TryParse(id, out i))
                {
                    PicsID.Add(i);
                }
            }
        }
        public static bool LoadCDB(string dir, bool overwrite, bool clearData = false)
        {
            logger.Info("Start LOAD CDB {0}", dir);
            LoadPicsFile();

            if (!File.Exists(dir))
                return false;

            if (clearData)
            {
                CardData.Clear();
            }

            SQLiteConnection connection = new SQLiteConnection("Data Source=" + dir);
            List<string[]> datas = new List<string[]>();
            List<string[]> texts = new List<string[]>();

            try
            {
                connection.Open();
                datas = SQLiteCommands.LoadData(connection);
                texts = SQLiteCommands.LoadText(connection);
                connection.Close();
            }
            catch (Exception ex)
            {
                logger.Error("LOADCDB :" + ex.ToString());
                connection.Close();
                return false;
            }

            progress = 0;
            total = datas.Count + texts.Count;
            foreach (string[] row in datas)
            {
                if (overwrite)
                    CardManager.UpdateOrAddCard(new CardInfos(row));
                else
                {
                    if (!CardManager.ContainsCard(int.Parse(row[0])))
                        CardManager.UpdateOrAddCard(new CardInfos(row));
                }

                if (!CheckPicsLoaded(Convert.ToInt32(row[0])))
                    DownloadPics(row[0]);
                else
                {
                    progress++;
                    Application.Current.Dispatcher.Invoke(() => LoadingProgress?.Invoke(progress, total));
                }
            }
            foreach (string[] row in texts)
            {
                progress++;
                Application.Current.Dispatcher.Invoke(() => LoadingProgress?.Invoke(progress, total));

                if (CardManager.ContainsCard(int.Parse(row[0])))
                    CardManager.GetCard(int.Parse(row[0])).SetCardText(row);
            }

            if (File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "strings.conf")))
                LoadSetCodesFromFile(CreateFileStreamFromString(File.ReadAllText(Path.Combine(FormExecution.path, "BattleCityAlpha", "strings.conf"))));
            SetCodesStringInit();

            while (progress != total)
                Thread.Sleep(500);
            Application.Current.Dispatcher.Invoke(() => LoadingFinished?.Invoke());

            return true;
        }

        public static CardInfos GetCard(int id)
        {
            if (CardData.ContainsKey(id))
                return CardData[id];
            return null;
        }

        public static bool RemoveCard(int id)
        {
            if (CardData.ContainsKey(id))
                return CardData.Remove(id);
            return false;
        }

        public static void UpdateOrAddCard(CardInfos card)
        {
            if (ContainsCard(card.Id))
                CardData[card.Id] = card;
            else
                CardData.Add(card.Id, card);
        }

        public static bool ContainsCard(int id)
        {
            return CardData.ContainsKey(id);
        }

        public static void RenameKey(int fromkey, int tokey)
        {
            CardData.RenameKey(fromkey, tokey);
        }

        public static Dictionary<int, CardInfos>.KeyCollection GetKeys()
        {
            return CardData.Keys;
        }

        public static int Count
        {
            get { return CardData.Count; }
        }

        public static List<CardInfos> GetCard(string name)
        {
            List<CardInfos> cards = new List<CardInfos>();

            foreach (var v in CardData)
                if (v.Value.Name.ToUpper().Contains(name.ToUpper()))
                {
                    if (v.Value.Name.ToUpper() == name.ToUpper())
                    {
                        cards.Clear();
                        cards.Add(v.Value);
                        return cards;
                    }
                    else
                        cards.Add(v.Value);
                }
            return cards;
        }
        public static List<CardInfos> GetCardBySetname(string name)
        {
            List<CardInfos> cards = new List<CardInfos>();
            int id = GetSetnameId(name);
            if (id == -1)
                return cards;

            foreach (var v in CardData)
                if (v.Value.BelongSetname(id))
                    cards.Add(v.Value);
            return cards;
        }
        public static int GetSetnameId(string name)
        {
            int id = -1;
            foreach (var v in SetCodes)
                if (v.Value.ToUpper() == name.ToUpper())
                    return v.Key;
            return id;
        }
        private static void SetCodesStringInit()
        {
            SetCodesString = new Dictionary<char, List<string>>();
            foreach (var v in SetCodes)
            {
                char startLetter = v.Value.ToUpper()[0];
                if (SetCodesString.ContainsKey(startLetter))
                {
                    List<string> list = SetCodesString[startLetter];
                    list.Add(v.Value);
                    SetCodesString[startLetter] = list;
                }
                else
                {
                    List<string> list = new List<string>();
                    list.Add(v.Value);
                    SetCodesString.Add(startLetter, list);
                }
            }
        }

        private static Stream CreateFileStreamFromString(string file)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(file);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        private static void LoadSetCodesFromFile(Stream file)
        {
            var reader = new StreamReader(file);
            while (!reader.EndOfStream)
            {
                //!setcode 0x8d Ghostrick
                string line = reader.ReadLine();
                if (line == null || !line.StartsWith("!setname")) continue;
                string[] parts = line.Split(' ');
                if (parts.Length == 1) continue;

                int setcode = Convert.ToInt32(parts[1], 16);
                string setname = line.Split(new string[] { parts[1] }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                if (!SetCodes.ContainsKey(setcode))
                    SetCodes.Add(setcode, setname);
                else
                    SetCodes[setcode] = setname;
            }
        }

        private static bool CheckPicsLoaded(int id)
        {
            return PicsID.Contains(id);
        }
        private static async void DownloadPics(string id)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileAsync(
                        GetUri(id),
                        Path.Combine(FormExecution.path, "BattleCityAlpha", "pics", id + ".jpg")
                        );
                    wc.DownloadFileCompleted += cardDownloaded;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Card error when downloading : " + id + " Ex : " + ex.ToString());
                total--;
            }
            await Task.Delay(1);
        }

        private static void cardDownloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            progress++;
            Application.Current.Dispatcher.Invoke(() => LoadingProgress?.Invoke(progress, total));
        }

        public static Uri GetUri(string id)
        {
            string s = "";
            if (!FormExecution.ClientConfig.BCA_Card_Design)
                s = string.Format("http://raw.githubusercontent.com/Battle-City-Alpha/Pics_BCA/master/base_design/{0}.jpg", id);
            else
                s = string.Format("http://raw.githubusercontent.com/Battle-City-Alpha/Pics_BCA/master/bca_design/{0}.jpg", id);
            return new Uri(s);
        }
    }
}