using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client.Configuration
{
    public static class AppTools
    {
        public static string Key = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefhijklmnopqrstuvwxyz0123456789";
        public static Random rd = new Random();

        public static string GenerateKey()
        {
            string salt = string.Empty;
            for (int i = 1; i <= 20; i++)
            {
                salt += Key[rd.Next(Key.Length)];
            }
            return salt;
        }
    }
}
