using BCA.Common.Enums;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using hub_client.Windows;
using System;
using System.Windows;

namespace hub_client.WindowsAdministrator
{
    public class DailyQuestAdministrator
    {
        public GameClient Client;

        public event Action<DailyQuestType, bool> ChangeQuestState;
        public event Action<DailyQuestType, string> ChangeQuest;

        public DailyQuestAdministrator(GameClient client)
        {
            Client = client;
            Client.GetDailyQuests += Client_GetDailyQuests;
            Client.DailyQuestReward += Client_DailyQuestReward;
            Client.ChangeDailyQuest += Client_ChangeDailyQuest;
        }

        private void Client_ChangeDailyQuest(bool success, DailyQuestType dqtype, string quest)
        {
            if (success)
                ChangeQuest?.Invoke(dqtype, quest);
            else
                FormExecution.Client_PopMessageBox("Tu as déjà utilisé ton changement de quête aujourd'hui ! (Un seul changement par jour)", "Quête journalière !");
        }

        private void Client_DailyQuestReward(bool success, DailyQuestType dqtype, int amount, bool allquest)
        {
            if (allquest)
                FormExecution.Client_PopMessageBox("Félicitations ! Tu as rempli toutes les quêtes possible aujourd'hui ! Tu remportes un bonus de 400 BPs supplémentaire !", "Quête journalière !");
            else
            {
                if (success)
                {
                    ChangeQuestState?.Invoke(dqtype, success);
                    FormExecution.Client_PopMessageBox("Félicitations ! La quête est complétée ! Tu remportes " + amount + " BPs." + Environment.NewLine + "Reviens demain pour de nouveaux défis !", "Quête journalière !");
                }
                else
                    FormExecution.Client_PopMessageBox("La quête n'est pas encore terminée ! Encore un petit effort...", "Quête journalière !");
            }
        }

        private void Client_GetDailyQuests(DailyQuestType[] dqtype, string[] quests, int[] states)
        {
            DailyQuestWindow window = new DailyQuestWindow(this, dqtype, quests, states);
            window.Show();
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => window.Activate()));
        }

        public void SendGetReward(DailyQuestType dqtype)
        {
            Client.Send(PacketType.GetDailyQuest, new StandardClientGetQuestReward
            {
                DQType = dqtype
            });
        }

        public void SendChangeDailyQuest(DailyQuestType dqtype)
        {
            Client.Send(PacketType.ChangeDailyQuest, new StandardClientChangeDailyQuest
            {
                DQType = dqtype
            });
        }
    }
}
