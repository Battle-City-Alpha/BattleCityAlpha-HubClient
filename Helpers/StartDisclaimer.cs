using System;

namespace hub_client.Helpers
{
    public static class StartDisclaimer
    {
        public static string RankedText = "Ce bouton te permet de jouer en mode classé ! Tu vas rejoindre une file d'attente et chercher un adversaire pour combattre. Il est important de savoir que chaque partie en classé rapporte le triple de gains (BP et expérience)." + Environment.NewLine + "ATTENTION : Lorsque le duel va se lancer, il se lancera directement avec le deck sélectionné par défaut sur la fenêtre principale (en bas à gauche), fais le bon choix !";
        public static string BrocanteText = "Vous venez d'accéder pour la première fois à la brocante ! C'est l'endroit où les joueurs peuvent vendre et acheter des cartes à d'autres joueurs. La brocante est réinitialisée tous les dimanches à minuit, date à laquelle tu récupéreras les cartes que tu as déposé si elles ne sont pas vendues.";
        public static string ShadowDuelText = "ATTENTION ! Vous vous apprêtez à faire/recevoir une demande pour un duel des ombres. Faites bien attention à la mise de ce dernier ! En effet, les mises peuvent être des BPs (le gagnant remporte ceux du perdant), un banissement (le perdu est banni pour la durée indiquée) ou un mute... Si vous acceptez le défi, soyez sûr d'en mesurer le risque !";
    }
}
