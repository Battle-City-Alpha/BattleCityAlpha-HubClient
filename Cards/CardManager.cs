﻿using Microsoft.Data.Sqlite;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static hub_client.Cards.SQLCommands;

namespace hub_client.Cards
{
    public static class CardManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Dictionary<int, CardInfos> CardData = new Dictionary<int, CardInfos>();
        public static Dictionary<int, string> SetCodes = new Dictionary<int, string>();
        public static Dictionary<char, List<string>> SetCodesString = new Dictionary<char, List<string>>();

        private static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
                              TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }

        public static async Task<bool> LoadCDB(string dir, bool overwrite, bool clearData = false)
        {
            if (!File.Exists(dir))
                return false;

            if (clearData)
            {
                CardData.Clear();
            }

            SqliteConnection connection = new SqliteConnection("Data Source=" + dir);
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

            foreach (string[] row in datas)
            {
                if (overwrite)
                    CardManager.UpdateOrAddCard(new CardInfos(row));
                else
                {
                    if (!CardManager.ContainsCard(Int32.Parse(row[0])))
                        CardManager.UpdateOrAddCard(new CardInfos(row));
                }

                if (!CheckPicsLoaded(row[0]))
                    await DownloadPics(row[0]);
            }
            foreach (string[] row in texts)
            {
                if (CardManager.ContainsCard(Int32.Parse(row[0])))
                    CardManager.GetCard(Int32.Parse(row[0])).SetCardText(row);
            }

            if (File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "strings.conf")))
                LoadSetCodesFromFile(CreateFileStreamFromString(File.ReadAllText(Path.Combine(FormExecution.path, "BattleCityAlpha", "strings.conf"))));
            SetCodesStringInit();

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

        private static bool CheckPicsLoaded(string id)
        {
            return File.Exists(Path.Combine(FormExecution.path, "BattleCityAlpha", "pics", id + ".jpg"));
        }
        private static async Task DownloadPics(string id)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileAsync(
                    GetUri(id),
                    Path.Combine(FormExecution.path, "BattleCityAlpha", "pics", id + ".jpg")
                    );
            }
            await Task.Delay(1);
        }
        public static Uri GetUri(string id)
        {
            string s = "";
            if (!FormExecution.ClientConfig.BCA_Card_Design)
                s = string.Format("https://raw.githubusercontent.com/Tic-Tac-Toc/Pics_BCA/master/base_design/{0}.jpg", id);
            else
                s = string.Format("https://raw.githubusercontent.com/Tic-Tac-Toc/Pics_BCA/master/bca_design/{0}.jpg", id);

            logger.Trace("URI : {0}", s);
            return new Uri(s);
        }
    }
}