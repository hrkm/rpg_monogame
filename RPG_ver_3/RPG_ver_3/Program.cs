#region Using Statements

using System;

#endregion

namespace RPG_ver_3
{
    /// <summary>
    /// G³ówna klasa aplikacji.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// G³ówny w¹tek aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
