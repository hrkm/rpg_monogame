#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace RPG_ver_0
{
    /// <summary>
    /// G��wna klasa aplikacji.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// G��wny w�tek aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}