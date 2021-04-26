using SPR.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour P_SubFolders.xaml
    /// </summary>
    public partial class P_SubFolders : Page
    {


        public SubFolderModel Model { get; }

        public P_SubFolders()
        {
            Model = new SubFolderModel();
            InitializeComponent();


        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = Model;
        }
    }
}
