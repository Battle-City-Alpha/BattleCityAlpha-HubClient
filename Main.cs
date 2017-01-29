using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hub_client
{
    class Main
    {
        public static Version VERSION = new Version(2, 0, 0, 0);

        public Main()
        {
            FormExecution.Init();
        }
    }
}
