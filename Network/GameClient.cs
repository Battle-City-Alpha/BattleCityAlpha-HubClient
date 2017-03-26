﻿using BCA.Network;
using BCA.Network.Packets;
using BCA.Network.Packets.Enums;
using BCA.Network.Packets.Standard.FromServer;
using hub_client.WindowsAdministrator;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace hub_client.Network
{
    public class GameClient : BinaryClient
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        private string _username;
        private string _password;

        public event Action<string, string> PopMessageBox;
        #region ChatForm Events
        public event Action<Color, string, bool, bool> ChatMessageRecieved;
        public event Action<string> AddHubPlayer;
        public event Action<string> RemoveHubPlayer;
        #endregion
        #region RegisterForm Events
        public event Action RegistrationComplete;
        #endregion
        #region LoginForm Events
        public event Action LoginComplete;
        #endregion

        #region Administrator
        public ChatAdministrator ChatAdmin;
        public RegisterAdminstrator RegisterAdmin;
        public LoginAdminstrator LoginAdmin;
        public NotesAdministrator NotesAdmin;
        #endregion

        public GameClient() : base(new NetworkClient())
        {
            PacketReceived += GameClient_PacketReceived;
            Disconnected += Client_Disconnected;
            InitAdministrator();
        }

        private void InitAdministrator()
        {
            ChatAdmin = new ChatAdministrator(this);
            RegisterAdmin = new RegisterAdminstrator(this);
            LoginAdmin = new LoginAdminstrator(this);
            NotesAdmin = new NotesAdministrator(this);
        }

        public void OpenPopBox(string text, string title)
        {
            logger.Trace("Open PopBox - Text : {0}, Title : {1}", text, title);
            Application.Current.Dispatcher.Invoke(() => PopMessageBox?.Invoke(text, title));
        }

        public string Username()
        {
            return _username;
        }

        public void InitPlayer(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private void Client_Disconnected(Exception ex)
        {
            logger.Fatal(ex);
            //TODO Display Message.
        }

        public void Send(PacketType packetId, Packet packet)
        {
            logger.Trace("PACKET SEND - {0} : {1}", packetId, packet);
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write((short)packetId);
                    writer.Write(JsonConvert.SerializeObject(packet));
                }
                Send(stream.ToArray());
            }
        }

        private void GameClient_PacketReceived(BinaryReader reader)
        {
            PacketType packetType = (PacketType)reader.ReadInt16();
            string packet = reader.ReadString();

            logger.Trace("PACKET RECEIVED - {0} : {1}", packetType, packet);

            switch (packetType)
            {
                case PacketType.ChatMessage:
                    OnChatMessage(JsonConvert.DeserializeObject<StandardServerChatMessage>(packet));
                    break;
                case PacketType.Register:
                    OnRegister(JsonConvert.DeserializeObject<StandardServerRegister>(packet));
                    break;
                case PacketType.Login:
                    OnLogin(JsonConvert.DeserializeObject<StandardServerLogin>(packet));
                    break;
                case PacketType.AddHubPlayer:
                    OnAddHubPlayer(JsonConvert.DeserializeObject<StandardServerAddHubPlayer>(packet));
                    break;
                case PacketType.RemoveHubPlayer:
                    OnRemoveHubPlayer(JsonConvert.DeserializeObject<StandardServerRemoveHubPlayer>(packet));
                    break;
                case PacketType.PlayerList:
                    OnUpdateHubPlayerList(JsonConvert.DeserializeObject<StandardServerPlayerlist>(packet));
                    break;
            }
        }

        private void OnChatMessage(StandardServerChatMessage packet)
        {
            ChatMessageType type = packet.Type;

            Color c;
            string msg;
            bool italic = false;
            bool bold = false;

            switch (type)
            {
                case ChatMessageType.Standard:
                    c = FormExecution.AppDesignConfig.StandardMessageColor;
                    msg = packet.Username + ":" + packet.Message;
                    break;
                case ChatMessageType.Animation:
                    c = FormExecution.AppDesignConfig.AnimationMessageColor;
                    msg = "[Animation - " + packet.Username + "]:" + packet.Message;
                    bold = true;
                    break;
                case ChatMessageType.Information:
                    c = FormExecution.AppDesignConfig.InformationMessageColor;
                    msg = "[Information - " + packet.Username + "]:" + packet.Message;
                    italic = true;
                    break;
                default:
                    c = FormExecution.AppDesignConfig.LauncherMessageColor;
                    msg = "••• Une erreur s'est produite.";
                    break;
            }

            ChatMessageRecieved?.Invoke(c, msg, italic, bold);
            logger.Trace("CHAT MESSAGE - Type : {0} | Username : {1} | Message : {2}", packet.Type, packet.Username, packet.Message);
        }

        private void OnRegister(StandardServerRegister packet)
        {
            if (!packet.Success)
            {
                switch (packet.Reason)
                {
                    case RegisterFailReason.EmailAlreadyUsed:
                        OpenPopBox("L'adresse email est déja utilisée.", "Problème");
                        break;
                    case RegisterFailReason.InvalidEmail:
                        OpenPopBox("L'adresse email est invalide.", "Problème");
                        break;
                    case RegisterFailReason.InvalidUsername:
                        OpenPopBox("Le nom d'utilisateur contient des caractères spéciaux interdits.", "Problème");
                        break;
                    case RegisterFailReason.UnknownProblem:
                        OpenPopBox("Erreur inconnue rencontrée.", "Problème");
                        break;
                    case RegisterFailReason.UsernameAlreadyUsed:
                        OpenPopBox("Le nom d'utilisateur est déja utilisée.", "Problème");
                        break;
                    case RegisterFailReason.UsernameTooLong:
                        OpenPopBox("Le nom d'utilisateur est trop long, il doit être inférieur à 20 caractères.", "Problème");
                        break;
                }
            }
            else
            {
                OpenPopBox("Votre inscription s'est bien déroulée. L'équipe de Battle City Alpha vous souhaite bon jeu et vous remercie d'avoir choisi notre plateforme.", "Bienvenue !");
                Application.Current.Dispatcher.Invoke(() => RegistrationComplete?.Invoke());
            }

            logger.Trace("REGISTER - Success : {0}, Reason : {1}", packet.Success, packet.Reason);
        }

        private void OnLogin(StandardServerLogin packet)
        {
            if (!packet.Success)
            {
                switch (packet.Reason)
                {
                    case LoginFailReason.Banned:
                        OpenPopBox("Vous êtes banni.", "Problème");
                        break;
                    case LoginFailReason.InvalidCombinaison:
                        OpenPopBox("La combinaison utilisateur/mot de passe est invalide.", "Problème");
                        break;
                    case LoginFailReason.UsernameDoesntExist:
                        OpenPopBox("Le nom d'utilisateur n'existe pas.", "Problème");
                        break;
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => LoginComplete?.Invoke());
            }

            logger.Trace("Login - Success : {0}, Reason : {1}", packet.Success, packet.Reason);
        }

        private void OnAddHubPlayer(StandardServerAddHubPlayer packet)
        {
            AddHubPlayer?.Invoke(packet.Username);
            logger.Trace("AddHubPlayer - {0}", packet.Username);
        }

        private void OnRemoveHubPlayer(StandardServerRemoveHubPlayer packet)
        {
            RemoveHubPlayer?.Invoke(packet.Username);
            logger.Trace("RemoveHubPlayer - {0}", packet.Username);
        }

        private void OnUpdateHubPlayerList(StandardServerPlayerlist packet)
        {
            foreach (string username in packet.Userlist)
                AddHubPlayer?.Invoke(username);
            logger.Trace("UpdatePlayerList - {0}", packet.Userlist);
        }

    }
}