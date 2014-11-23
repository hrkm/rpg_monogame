using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RPG_ver_5.WP8.Resources;
using MonoGame.Framework.WindowsPhone;

namespace RPG_ver_5.WP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Game1 _game;

        public MainPage()
        {
            InitializeComponent();

            _game = XamlGame<Game1>.Create("", this);
        }
    }
}