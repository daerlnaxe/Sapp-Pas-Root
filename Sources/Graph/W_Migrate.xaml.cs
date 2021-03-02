using DxTBoxCore.BoxChoose;
using DxTBoxCore.Languages;
using SPR.Languages;
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
using System.Windows.Shapes;
using Unbroken.LaunchBox.Plugins.Data;

namespace SPR.Graph
{
    /// <summary>
    /// Logique d'interaction pour W_Migrate.xaml
    /// </summary>
    public partial class W_Migrate : Window
    {
        MigrateModel _Model;

        public bool ActiveBox { get; set; }

        public W_Migrate(IPlatform selectedPlatform)
        {
            _Model = new MigrateModel(new Page())
            {

            };
            InitializeComponent();

           // _Model.InitializeEdition(selectedPlatform);
            this.DataContext = _Model;
        }




    }
}
