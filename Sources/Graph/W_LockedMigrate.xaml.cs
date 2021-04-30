using SPR.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SPR.Graph
{
    /// <summary>
    /// Migre des fichiers entre une ancienne plateforme et une nouvelle
    /// </summary>
    public partial class W_LockedMigrate : Window
    {
        public MigrateLModel Model { get; internal set; }

        public W_LockedMigrate()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Model;
        }


        private void Exec_Apply(object sender, ExecutedRoutedEventArgs e)
        {
            Model.Exec();
        }


    }
}
