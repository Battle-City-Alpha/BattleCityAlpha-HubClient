using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromClient;
using hub_client.Network;
using System;

namespace hub_client.WindowsAdministrator
{
    public class DataRetrievalAdministrator
    {
        public GameClient Client;

        public event Action StartLoading;
        public event Action EndLoading;

        public DataRetrievalAdministrator(GameClient client)
        {
            Client = client;

            Client.DataRetrievalInfos += Client_DataRetrievalInfos;
        }

        private void Client_DataRetrievalInfos(bool success, LoginFailReason reason, bool end)
        {
            if (success)
            {
                if (end)
                {
                    EndLoading?.Invoke();
                    FormExecution.Client_PopMessageBox("La récupération de données est terminée !", "Récupération de données");
                }
                else
                    StartLoading?.Invoke();
            }
            else
            {
                EndLoading?.Invoke();
                string error = "Une erreur s'est produite lors de la récupération : ";
                switch (reason)
                {
                    case LoginFailReason.InvalidCombinaison:
                        error += "La combinaison pseudo/mot de passe n'est pas bonne. Mauvais mot de passe ou pseudo inexistant sur la v1";
                        break;
                    case LoginFailReason.DataRetrievalAlreadyMade:
                        error += "Tu as déjà récupéré tes données !";
                        break;
                }
                FormExecution.Client_PopMessageBox(error, "Récupération de données");
            }
        }

        public void SendAskDataRetrieval(string username, string password)
        {
            Client.Send(PacketType.DataRetrieval, new StandardClientAskDataRetrieval
            {
                Username = username,
                Password = password
            });
        }
    }
}
