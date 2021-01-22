using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal class Env
    {
        /// <summary>
        /// Flag to read data on startup.
        /// </summary>
        internal bool dataReadOnStartup = true;
        private static Env sEnv;

        internal static Env getEnv()
        {
            if (sEnv == null)
                new Env();
            return sEnv;
        }

        internal Env()
        {
            sEnv = this;
        }
    }
}
